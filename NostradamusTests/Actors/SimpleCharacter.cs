using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Snapshots;
using System;

namespace Nostradamus.Tests.Actors
{
	class SimpleCharacter : Actor
	{
		public SimpleCharacter(Scene scene, ActorId id, int time, ISnapshotArgs snapshot)
			: base(scene, id, time, snapshot)
		{ }

		internal protected override void OnCommand(ISnapshotArgs snapshot, ICommandArgs command)
		{
			if (command is MoveActorCommand)
			{
				var s = (ActorSnapshot)snapshot;
				var c = (MoveActorCommand)command;

				var e = new ActorMovedEvent()
				{
					PositionX = s.PositionX + c.DeltaX,
					PositionY = s.PositionY + c.DeltaY,
				};

				ApplyEvent(e);
			}
			else
				throw new NotSupportedException(command.GetType().FullName);
		}

		internal protected override void OnEvent(ISnapshotArgs snapshot, IEventArgs @event)
		{
			if (@event is ActorMovedEvent)
			{
				var s = (ActorSnapshot)snapshot;
				var e = (ActorMovedEvent)@event;

				s.PositionX = e.PositionX;
				s.PositionY = e.PositionY;
			}
			else
				throw new NotSupportedException(@event.GetType().FullName);
		}
	}
}
