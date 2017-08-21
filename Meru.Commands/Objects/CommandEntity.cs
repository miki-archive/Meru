using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandEntity
    {
        public List<CommandEntity> Children = new List<CommandEntity>();

        public string Id = "";

        public CommandEntity Parent = null;

        public CommandEntity()
        {
            
        }
        public CommandEntity(CommandEntity entity)
        {
            Id = entity.Id;
            Parent = entity.Parent;
            Children = entity.Children;
        }

        public Dictionary<string, T> GetAllEntitiesOf<T>(bool includeSubmodules = true) where T : CommandEntity
        {
            Dictionary<string, T> allEntities = new Dictionary<string, T>();

            foreach (CommandEntity entity in Children)
            {
                if (includeSubmodules)
                {
                    if (entity is Module m)
                    {
                        Dictionary<string, T> tempDictionary = m.GetAllEntitiesOf<T>(includeSubmodules);
                        foreach (var kv in tempDictionary)
                        {
                            allEntities.Add(kv.Key.ToLower(), kv.Value);
                        }
                    }

                    if (entity is T t)
                    {
                        allEntities.Add(entity.Id, t);
                    }
                }
            }

            return allEntities;
        }
    }
}
