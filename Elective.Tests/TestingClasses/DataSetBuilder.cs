using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elective.Tests.TestingClasses
{
	class DataSetBuilder<TModel>
		where TModel : class, new()
	{
		public static List<TModel> Build() => Build(3);

		public static List<TModel> Build(int modelsAmmount)
		{
			var dataSet = new List<TModel>();
			for(int i = 0; i<modelsAmmount; i++)
			{
				dataSet.Add(new TModel());
			}
			return dataSet;
		}
	}
}
