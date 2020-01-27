using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using REST;
using Common.Extensions;
using System.Linq;
using SharedTest;
using Common;

namespace Patching
{
	public abstract class Patching<T>
	{
		protected static Random Random = new Random();
		protected abstract IEnumerable<T> SampleValues { get; }

		[ViewClass]
		public struct ComplexStruct
		{
			public T Field;
			
			public string StringVal;
			public int IntVal;
			public double DoubleVal;
			public GeoLocation LocationVal;

			public ComplexStruct(T val)
			{
				Field = val;
				StringVal = StringExtensions.RandomString(64);
				IntVal = Random.Next(0, int.MaxValue);
				DoubleVal = Random.NextDouble();
				LocationVal = new GeoLocation(Random.NextDouble() * 90, Random.NextDouble() * 90);
			}
		}

		public class ComplexClass
		{
			[View(EPermissionLevel.PUBLIC)]
			public T Property { get; set; }
			[View(EPermissionLevel.PUBLIC)]
			public T Field;
			[View(EPermissionLevel.PUBLIC)]
			public string StringVal;
			[View(EPermissionLevel.PUBLIC)]
			public int IntVal;
			[View(EPermissionLevel.PUBLIC)]
			public double DoubleVal;
			[View(EPermissionLevel.PUBLIC)]
			public GeoLocation LocationVal;

			public ComplexClass(T val)
			{
				Field = val;
				Property = val;
				StringVal = StringExtensions.RandomString(64);
				IntVal = Random.Next(0, int.MaxValue);
				DoubleVal = Random.NextDouble();
				LocationVal = new GeoLocation(Random.NextDouble() * 90, Random.NextDouble() * 90);
			}
		}

		[TestMethod]
		public void PatchFieldInComplexClass()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Field", value2}
				};
				var cl2 = new ComplexClass(value1);
				cl2.PatchObject(patch);
				Assert.AreEqual(value2, cl2.Field);
			}
		}

		public class ClassWithList
		{
			[View(EPermissionLevel.PUBLIC)]
			public List<T> List;
		}

		[TestMethod]
		public void PatchList()
		{
			var originalList = SampleValues;
			var objWithList = new ClassWithList()
			{
				List = new List<T>(SampleValues)
			};
			var newList = originalList.OrderBy(_ => Random.Next()).ToList();
			var patch = new Dictionary<string, object>()
			{
				{ "List", newList }
			};
			objWithList.PatchObject(patch);
			Assert.IsTrue(objWithList.List.SequenceEqual(newList));
		}

		[TestMethod]
		public void PatchPropertyInComplexClass()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Property", value2}
				};
				var cl2 = new ComplexClass(value1);
				cl2.PatchObject(patch);
				Assert.AreEqual(value2, cl2.Property);
			}
		}

		[ViewClass]
		struct SingleFieldStructWithViewClass
		{
			public T Value;
		}

		struct SingleFieldStructWithViewAttr
		{
			[View(EPermissionLevel.PUBLIC)]
			public T Value;
		}

		class SingleFieldClassWithViewAttr
		{
			[View(EPermissionLevel.PUBLIC)]
			public T Value;
		}

		[TestMethod]
		public void PatchFieldInSimpleClass()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var cl2 = new SingleFieldClassWithViewAttr { Value = value1 };
				cl2.PatchObject(patch);
				Assert.AreEqual(value2, cl2.Value);
			}
		}

		[TestMethod]
		public void PatchStruct()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach(var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var str = new SingleFieldStructWithViewClass { Value = value1 };
				str = str.PatchObject(patch);
				Assert.AreEqual(value2, str.Value);
			}
		}

		[TestMethod]
		public void CannotMutateStruct()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var str2 = new SingleFieldStructWithViewAttr { Value = value1 };
				AssertX.Throws<Exception>(() => str2.PatchObject(patch), e => e.Message.Contains("Cannot patch immutable struct - you must pass the full object."));
			}
		}
	}
}
