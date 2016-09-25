using System;

namespace Nostradamus
{
	public abstract class PredictiveActor : AuthoritativeActor
	{
		private Timeline predictiveTimeline;
		private CommandQueue commands = new CommandQueue();

		protected PredictiveActor(Scene scene, ActorId id)
			: base( scene, id )
		{
			predictiveTimeline = new Timeline( string.Format( "Predictive-{0}", commands.MaxCommandSequence ) );
		}

		internal void CreatePredictiveTimeline(int predictiveTime, int authoritativeTime)
		{
			var timepoint = AuthoritativeTimeline.FindPoint( authoritativeTime );
			if( timepoint == null )
				throw new ArgumentException( string.Format( "Cannot find authoritative timepoint at {0}", predictiveTime ) );

			predictiveTimeline = new Timeline( string.Format( "Predictive-{0}", predictiveTime ) );
		}

		internal void AddPredictiveCommand(int time, ICommandArgs commandArgs)
		{
			commands.Enqueue( time, commandArgs );
		}

		internal void AddPredictivePoint(int time, ISnapshotArgs snapshot)
		{
			if( predictiveTimeline == null )
				throw new ArgumentNullException( "predictiveTimeline" );

			predictiveTimeline.AddPoint( time, snapshot );
		}

		internal Timeline PredictiveTimeline
		{
			get { return predictiveTimeline; }
		}
	}
}
