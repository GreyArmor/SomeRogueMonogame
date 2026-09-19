using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Map;
using System;

namespace NamelessRogue.Engine.Factories
{
    public class GameInitializer {

        public static Entity CreateCursor()
        {
            Entity cursor = new Entity();
            cursor.AddComponent(new Cursor());
            cursor.AddComponent(new Position(0,0,0));
            Drawable dr = new Drawable("Cursor", new Engine.Utility.Color(0.9,0.9,0.9));
            dr.Visible = false;
            cursor.AddComponent(dr);
            cursor.AddComponent(new InputComponent());
            cursor.AddComponent(new LineToPlayer());
            return cursor;
        }

        public static Entity CreateTargeter()
        {
            Entity cursor = new Entity();
            cursor.AddComponent(new TergeterComponent());
            return cursor;
        }

        internal static Entity CreateWorldMapCamera()
        {
            Entity cameraEntity = new Entity();
            cameraEntity.AddComponent(new WorldMapCameraComponent(new System.Numerics.Vector2(), 64));
            return cameraEntity;
        }

		internal static Entity CreateStreetlightsStatusKeeper()
		{
			Entity streetLightKeeper = new Entity();
			streetLightKeeper.AddComponent(new StreetlightsStatusKeeper(){ VerticalMovementAllowed = true} );
			return streetLightKeeper;
		}
	}
}
