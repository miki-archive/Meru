using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandEntity
    {
        public List<CommandEntity> Children { get; set; } = new List<CommandEntity>();

        public string Id { get; set; }

        public CommandEntity Parent { get; set; }

        public CommandEntity()
        {            
        }
        public CommandEntity(CommandEntity entity)
        {
            Id = entity.Id;
            Parent = entity.Parent;
            Children = entity.Children;
        }

        public Dictionary<string, T> GetAllEntitiesOf<T>(bool recursive = true) where T : CommandEntity
        {
            Dictionary<string, T> allEntities = new Dictionary<string, T>();

            foreach (CommandEntity entity in Children)
            {
                if (recursive)
                {
                    if (entity is Module m)
                    {
                        Dictionary<string, T> tempDictionary = m.GetAllEntitiesOf<T>(recursive);
                        foreach (var kv in tempDictionary)
                        {
                            allEntities.Add(kv.Key.ToLower(), kv.Value);
                        }
                    }
                }

				if (entity is T t)
				{
					allEntities.Add(entity.Id, t);
				}
			}

            return allEntities;
        }
    }
}
