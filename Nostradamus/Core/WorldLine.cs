namespace Nostradamus
{
	class WorldLine
	{
		public Branch CreateBranch(string name)
		{
			return new Branch( name, null );
		}

		public Branch CreateBranch(string name, Node parentNode)
		{
			return new Branch( name, parentNode );
		}
	}
}
