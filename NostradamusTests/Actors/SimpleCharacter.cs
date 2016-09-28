using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Snapshots;
using System;

namespace Nostradamus.Tests.Actors
{
	class SimpleCharacter : Actor
	{
		public SimpleCharacter(Scene scene, ActorId id, ClientId clientId, ISnapshotArgs snapshot)
			: base(scene, id, clientId, snapshot)
		{ }

		protected internal override void OnCommandReceived(ICommandArgs command)
		{
			if (command is MoveActorCommand)
			{
				var s = (ActorSnapshot)Snapshot;
				if (s.HasMoved)
					return;

				var c = (MoveActorCommand)command;

				var e = new ActorMovedEvent()
				{
					PositionX = s.PositionX + c.DeltaX * Scene.DeltaTime / 1000f,
					PositionY = s.PositionY + c.DeltaY * Scene.DeltaTime / 1000f,
				};

				ApplyEvent(e);
			}
			else
				throw new NotSupportedException(command.GetType().FullName);
		}

		protected internal override ISnapshotArgs OnEventApplied(IEventArgs @event)
		{
			if (@event is ActorMovedEvent)
			{
				var s = (ActorSnapshot)Snapshot.Clone();
				var e = (ActorMovedEvent)@event;

				s.PositionX = e.PositionX;
				s.PositionY = e.PositionY;
				s.HasMoved = true;

				return s;
			}
			else
				throw new NotSupportedException(@event.GetType().FullName);
		}

		protected internal override void OnUpdate()
		{
			var s = (ActorSnapshot)Snapshot;

			s.HasMoved = false;
		}
	}
}
