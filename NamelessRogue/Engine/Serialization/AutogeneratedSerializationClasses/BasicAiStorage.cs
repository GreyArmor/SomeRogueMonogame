
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class BasicAiStorage : IStorage<NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public IList<PointStorage> Route { get; set; }

        [FlatBufferItem(3)]  public BasicAiStatesStorage State { get; set; }

        [FlatBufferItem(4)]  public PointStorage DestinationPoint { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {
            
            this.Route = new List<PointStorage>(component.Route.Select(x=>(PointStorage)x));

            this.State = (BasicAiStatesStorage)component.State;

            this.DestinationPoint = component.DestinationPoint;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {

            component.Route = new Queue<Point>(this.Route.Select(x=>(Point)x));

            component.State = (NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAiStates)this.State;

            component.DestinationPoint = this.DestinationPoint;

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi (BasicAiStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi result = new NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BasicAiStorage (NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi  component)
        {
            if(component == null) { return null; }
            BasicAiStorage result = new BasicAiStorage();
            result.FillFrom(component);
            return result;
        }

    }

}