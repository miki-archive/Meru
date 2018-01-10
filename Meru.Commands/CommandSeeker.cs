using Meru.Commands.Attributes;
using Meru.Commands.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Commands
{
    class CommandSeeker
    {
        public CommandEntity GetCommandsFromAttributeAsync()
        {
            CommandEntity commandHierarchyList = new CommandEntity();
            commandHierarchyList.Id = "root";

            Assembly asm = Assembly.GetEntryAssembly();

            var modules = asm.GetTypes()
                .Where(m => m.GetTypeInfo().GetCustomAttributes<ModuleAttribute>().Any() && !m.IsNested)
                .ToArray();

            int modulesLoaded = 0;
            int subModulesLoaded = 0;
            int commandsLoaded = 0;

            foreach (var m in modules)
            {
                CommandEntity entity = GetChildrenFromModule(m);
                commandHierarchyList.Children.Add(entity);

                modulesLoaded++;

                subModulesLoaded += entity.Children.Count(x => (x as Module) != null);
                commandsLoaded += entity.GetAllEntitiesOf<Command>().Count;
            }
            return commandHierarchyList;
        }

        public CommandEntity GetChildrenFromModule(Type attribute)
        {
            CommandEntity module = new CommandEntity();
            ModuleAttribute moduleAttribute = attribute.GetTypeInfo().GetCustomAttribute<ModuleAttribute>();

            module = new Module(moduleAttribute.Entity as Module);

            object constructedInstance = null;

           List<Type> allChildren = attribute.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
                .Where((x) => x.GetTypeInfo().GetCustomAttributes<CommandEntityAttribute>().Any())
                .ToList();

            try
            {
                constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName), module);
            }
            catch
            {
                constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName));
            }

            foreach (Type t in allChildren)
            {
                CommandEntityAttribute entityAttribute = t.GetTypeInfo().GetCustomAttribute<CommandEntityAttribute>();
				CommandEntity entity = null;

				if (entityAttribute is ModuleAttribute mAttribute)
                {
                    entity = GetChildrenFromModule(t);
                }
				else if(entityAttribute is MultiCommandAttribute cAttribute)
				{
					entity = ParseMultiCommand(t);
				}

				if (entity == null)
					continue;

				entity.Parent = module;
				module.Children.Add(entity);
			}

            List<MethodInfo> methods = attribute.GetMethods()
                .Where((x) => x.GetCustomAttributes<CommandAttribute>().Any())
                .ToList();

            foreach (MethodInfo m in methods)
            {
                module.Children.Add(CreateCommand(m, module, constructedInstance));
            }
            return module;
        }

		public CommandEntity ParseMultiCommand(Type attribute)
		{
			CommandEntity command = new CommandEntity();
			MultiCommandAttribute moduleAttribute = attribute.GetTypeInfo().GetCustomAttribute<MultiCommandAttribute>();

			command = new MultiCommand(moduleAttribute.Entity as MultiCommand);

			object constructedInstance = null;

			try
			{
				constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName), command);
			}
			catch
			{
				constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName));
			}

			List<Type> allChildren = attribute.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
				.Where((x) => x.GetTypeInfo().GetCustomAttributes<CommandEntityAttribute>().Any())
				.ToList();

			foreach (Type t in allChildren)
			{
				CommandEntityAttribute entityAttribute = t.GetTypeInfo().GetCustomAttribute<CommandEntityAttribute>();
				CommandEntity entity = null;

				if (entityAttribute is MultiCommandAttribute cAttribute)
				{
					entity = ParseMultiCommand(t);
				}

				if (entity == null)
					continue;

				entity.Parent = command;
				command.Children.Add(entity);
			}

			List<MethodInfo> methods = attribute.GetMethods()
				.Where((x) => x.GetCustomAttributes<CommandAttribute>().Any())
				.ToList();

			foreach (MethodInfo m in methods)
			{
				Command newEvent = CreateCommand(m, command, constructedInstance);

				if (newEvent.IsDefault && (command as MultiCommand).defaultCommand == null)
				{
					(command as MultiCommand).defaultCommand = newEvent;
				}

				command.Children.Add(newEvent);
			}
			return command;
		}

		public Command CreateCommand(MethodInfo m, CommandEntity parent, object constructedInstance)
		{
			CommandAttribute commandAttribute = m.GetCustomAttribute<CommandAttribute>();
			Command newEvent = new Command(commandAttribute.Entity as Command);

			newEvent.ProcessCommand =
				async (context) => await (Task)m.Invoke(constructedInstance, new object[] { context });
			newEvent.Parent = parent;
			return newEvent;
		}
	}
}
