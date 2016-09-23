using System;
using System.Collections.Generic;

namespace Nostradamus
{
	class Branch
	{
		private readonly string name;
		private readonly Node parentNode;
		private readonly LinkedList<Node> nodes = new LinkedList<Node>();

		internal Branch(string name, Node parentNode)
		{
			this.name = name;
			this.parentNode = parentNode;
		}

		public Node CreateNode(int time)
		{
			if( nodes.Last != null )
			{
				if( time <= nodes.Last.Value.Time )
					throw new ArgumentException( string.Format( "'time' must > {0}", nodes.Last.Value.Time ) );
			}
			else if( parentNode != null )
			{
				if( time <= parentNode.Time )
					throw new ArgumentException( string.Format( "'time' must > {0}", parentNode.Time ) );
			}

			var node = new Node( this, time );

			nodes.AddLast( node );

			return node;
		}

		public Node FindBefore(int time)
		{
			for( var node = nodes.Last; node != null; node = node.Previous )
			{
				if( node.Value.Time <= time )
					return node.Value;
			}

			return null;
		}

		public Node FindAfter(int time)
		{
			for( var node = nodes.First; node != null; node = node.Next )
			{
				if( node.Value.Time > time )
					return node.Value;
			}

			return null;
		}

		public Node First
		{
			get
			{
				if( parentNode != null )
					return parentNode;
				else
					return nodes.First != null ? nodes.First.Value : null;
			}
		}

		public Node Last
		{
			get { return nodes.Last != null ? nodes.Last.Value : null; }
		}
	}
}
