
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class BasicAiStorage : IStorage<NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Queue<PointStorage> Route { get; set; }

        [FlatBufferItem(3)]  public BasicAiStatesStorage State { get; set; }

        [FlatBufferItem(4)]  public PointStorage DestinationPoint { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {

            this.Route = new Queue<PointStorage>(component.Route.Cast<PointStorage>());

            this.State = (BasicAiStatesStorage)component.State;

            this.DestinationPoint = component.DestinationPoint;

            this.Id = component.Id;

            this.ParentEntityId = component.ParentEntityId;

        }

        public void FillTo(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {

            component.Route = new Queue<Microsoft.Xna.Framework.Point>(this.Route.Cast<Microsoft.Xna.Framework.Point>());

            component.State = (NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAiStates)this.State;

            component.DestinationPoint = this.DestinationPoint;

            component.Id = this.Id;

            component.ParentEntityId = this.ParentEntityId;


        }

        public static implicit operator NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi (BasicAiStorage thisType)
        {
            NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi result = new NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BasicAiStorage (NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi  component)
        {
            BasicAiStorage result = new BasicAiStorage();
            result.FillFrom(result);
            return result;
        }

    }

}