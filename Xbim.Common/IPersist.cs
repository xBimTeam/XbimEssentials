namespace Xbim.Common
{
	public interface IPersist
	{
		//This function is used by parsers to read in serialized data
		void Parse(int propIndex, IPropertyValue value, int[] nested);
	}
}