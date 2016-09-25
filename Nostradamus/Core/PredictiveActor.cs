using System;

namespace Nostradamus
{
	public abstract class PredictiveActor : AuthoritativeActor
	{
		private Timeline predictiveTimeline;
		private CommandQueue commands = new CommandQueue();

		protected PredictiveActor(Scene scene)
			: base( scene )
		{
			predictiveTimeline = new Timeline( string.Format( "Predictive-{0}", commands.MaxCommandSequence ) );
		}

		//internal override Timepoint AddAuthoritativePoint(int time, int lastCommandSeq, ISnapshotArgs snapshot)
		//{
		//	var authoritativeNode = base.AddAuthoritativePoint( time, lastCommandSeq, snapshot );

		//	if( lastCommandSeq > 0 )
		//	{
		//		var command = commands.Dequeue( lastCommandSeq );
		//		if( command != null )
		//		{
		//			var predictiveNode = predictiveBranch.FindAfter( command.Time );
		//			if( predictiveNode == null )
		//				throw new InvalidOperationException( "Failed to find node of command" );

		//			// TODO: Rewind only error happens
		//			var branchName = string.Format( "Predictive-{0}-Fix", lastCommandSeq );
		//			var lastPredictiveNode = predictiveBranch.Last;
		//			var newPredictiveBranch = WorldLine.CreateBranch( branchName, lastPredictiveNode );
		//		}
		//	}

		//	return authoritativeNode;
		//}

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
