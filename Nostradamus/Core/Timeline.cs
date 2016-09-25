using System;
using System.Collections.Generic;

namespace Nostradamus
{
	class Timeline
	{
		private readonly string name;
		private readonly LinkedList<Timepoint> points = new LinkedList<Timepoint>();

		internal Timeline(string name)
		{
			this.name = name;
		}

		public Timepoint AddPoint(int time, ISnapshotArgs snapshot)
		{
			if( points.Last != null )
			{
				if( time <= points.Last.Value.Time )
					throw new ArgumentException( string.Format( "'time' must > {0}", points.Last.Value.Time ) );
			}

			var point = new Timepoint( this, time, snapshot );

			points.AddLast( point );

			return point;
		}

		internal Timepoint FindPoint(int time)
		{
			for( var point = points.First; point != null; point = point.Next )
			{
				if( point.Value.Time == time )
					return point.Value;
			}

			return null;
		}

		public Timepoint FindBefore(int time)
		{
			for( var node = points.Last; node != null; node = node.Previous )
			{
				if( node.Value.Time <= time )
					return node.Value;
			}

			return null;
		}

		public Timepoint FindAfter(int time)
		{
			for( var point = points.First; point != null; point = point.Next )
			{
				if( point.Value.Time > time )
					return point.Value;
			}

			return null;
		}

		public Timepoint First
		{
			get { return points.First != null ? points.First.Value : null; }
		}

		public Timepoint Last
		{
			get { return points.Last != null ? points.Last.Value : null; }
		}
	}
}
