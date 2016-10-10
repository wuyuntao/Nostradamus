﻿using System;

namespace Nostradamus.Examples
{
    public class SimpleCharacter : Actor
    {
        private bool hasMoved;

        public SimpleCharacter(Scene scene, ActorId id, ClientId ownerId, ISnapshotArgs snapshot)
            : base(scene, id, ownerId, snapshot)
        { }

        protected internal override void OnCommandReceived(ICommandArgs command)
        {
            if (command is MoveCharacterCommand)
            {
                if (hasMoved)
                    return;

                var s = (CharacterSnapshot)Snapshot;
                var c = (MoveCharacterCommand)command;

                var e = new CharacterMovedEvent()
                {
                    PositionX = s.PositionX + c.DeltaX * Scene.DeltaTime / 1000f,
                    PositionY = s.PositionY + c.DeltaY * Scene.DeltaTime / 1000f,
                };

                ApplyEvent(e);

                hasMoved = true;
            }
            else
                base.OnCommandReceived(command);
        }

        protected internal override ISnapshotArgs OnEventApplied(IEventArgs @event)
        {
            if (@event is CharacterMovedEvent)
            {
                var s = (CharacterSnapshot)Snapshot.Clone();
                var e = (CharacterMovedEvent)@event;

                s.PositionX = e.PositionX;
                s.PositionY = e.PositionY;

                return s;
            }
            else
                return base.OnEventApplied(@event);
        }

        protected internal override void OnUpdate()
        {
            hasMoved = false;
        }
    }
}
