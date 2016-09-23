using System;

namespace Nostradamus
{
	public abstract class PredictiveActor : AuthoritativeActor
	{
		Branch predictiveBranch;
		CommandQueue commands = new CommandQueue();

		protected PredictiveActor(Scene scene)
			: base( scene )
		{ }

		public override void OnAuthoritativeEvent(int time, int lastCommandSeq, params IEventArgs[] events)
		{
			base.OnAuthoritativeEvent( time, lastCommandSeq, events );

			if( lastCommandSeq > 0 )
			{
				var command = commands.Dequeue( lastCommandSeq );
				if( command != null )
				{
					var node = predictiveBranch.FindAfter( command.Time );

					// TODO compare difference between authoritative branch and predictive branch
					throw new NotImplementedException();
				}
			}
		}

		public virtual void OnPredictiveCommand(int time, ICommandArgs commandArgs)
		{
			var command = commands.Enqueue( time, commandArgs );

			if( predictiveBranch == null )
			{
				var branchName = string.Format( "Predictive-{0}", command.Sequence );
				var lastAuthoritativeNode = AuthoritativeBranch.FindBefore( time );

				predictiveBranch = new Branch( branchName, lastAuthoritativeNode );
			}
		}

		public virtual void OnPredictiveEvent(int time, params IEventArgs[] events)
		{
			var node = predictiveBranch.CreateNode( time );

			foreach( var e in events )
				node.AddEvent( e );
		}

		internal Branch PredictiveBranch
		{
			get { return predictiveBranch; }
		}
	}
}
