using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Snapshots;
using System;

namespace Nostradamus.Tests.Actors
{
	class SimpleCharacter : Actor
	{
		private bool hasMoved;

		public SimpleCharacter(Scene scene, ActorId id, ISnapshotArgs snapshot)
			: base(scene, id, snapshot)
		{ }

		protected override void OnSnapshotRecovered(ISnapshotArgs snapshot)
		{
		}

		protected override void OnCommandReceived(ICommandArgs command)
		{
			if (command is MoveActorCommand)
			{
				if (hasMoved)
					return;

				var s = (ActorSnapshot)Snapshot;
				var c = (MoveActorCommand)command;

				var e = new ActorMovedEvent()
				{
					PositionX = s.PositionX + c.DeltaX * Scene.DeltaTime / 1000f,
					PositionY = s.PositionY + c.DeltaY * Scene.DeltaTime / 1000f,
				};

				ApplyEvent(e);

				hasMoved = true;
			}
			else
				throw new NotSupportedException(command.GetType().FullName);
		}

		protected override ISnapshotArgs OnEventApplied(IEventArgs @event)
		{
			if (@event is ActorMovedEvent)
			{
				var s = (ActorSnapshot)Snapshot.Clone();
				var e = (ActorMovedEvent)@event;

				s.PositionX = e.PositionX;
				s.PositionY = e.PositionY;

				return s;
			}
			else
				throw new NotSupportedException(@event.GetType().FullName);
		}

		protected override void OnUpdate()
		{
			hasMoved = false;
		}
	}
}
