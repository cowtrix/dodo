using System.Linq;
using System.IO;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.WorkingGroups;
using Dodo.Roles;
using Dodo.LocationResources;
using Dodo;
using Common.Commands;
using Common;

namespace DodoAOT
{
	/// <summary>
	/// Is this a dumb way to do things? Maybe. But hey, it works.
	/// I basically just wanted to make something that programatically generated the viewmodels
	/// and view razor pages, because I'm lazy and really it's all just derived from the base 
	/// data model. 
	/// It's basically just a super simple templating thing that swaps out some magic strings
	/// in the template files (see `Templates` folder) for their reflected values.
	/// We can probably rip this out once the CMS stuff stabilises a bit.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Info($"Generating AOT");
			var parsedArgs = new CommandArguments(args);
			var viewModelPath = Path.GetFullPath(parsedArgs.MustGetValue<string>("viewmodels"));
			Logger.Info($"Outputting viewmodels to {viewModelPath}");
			if (!Directory.Exists(viewModelPath))
			{
				throw new DirectoryNotFoundException(viewModelPath);
			}
			foreach (var rmType in 
				new[] 
				{
					typeof(Rebellion),
					typeof(LocalGroup),
					typeof(WorkingGroup),
					typeof(Role),
					typeof(Event),
					typeof(Site)
				})
			{
				using (var fs = new StreamWriter(Path.Combine(viewModelPath, $"{rmType.Name}.cs")))
				{
					fs.Write(ViewModelGenerator.Generate(rmType));
				}
			}

			// Create Views
			var viewPath = Path.GetFullPath(parsedArgs.MustGetValue<string>("views"));
			Logger.Info($"Outputting view .cshtml to {viewPath}");
			if (!Directory.Exists(viewPath))
			{
				throw new DirectoryNotFoundException(viewPath);
			}
			foreach (var rmType in
				new[]
				{
					typeof(Rebellion),
					typeof(LocalGroup),
					typeof(WorkingGroup),
					typeof(Role),
					typeof(Event),
					typeof(Site)
				})
			{
				var folderPath = Path.Combine(viewPath, rmType.Name);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				OutputReadonlyFile(CreateViewGenerator.Generate(rmType), Path.Combine(folderPath, $"Create.cshtml"));
				OutputReadonlyFile(EditViewGenerator.Generate(rmType), Path.Combine(folderPath, $"Edit.cshtml"));
				OutputReadonlyFile(DeleteViewGenerator.Generate(rmType), Path.Combine(folderPath, $"Delete.cshtml"));
			}
		}

		static void OutputReadonlyFile(string fileContents, string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.IsReadOnly)
			{
				fileInfo.IsReadOnly = false;
			}
			using (var fs = new StreamWriter(path))
			{
				fs.Write(fileContents);
			}
			fileInfo = new FileInfo(path);
			if (!fileInfo.IsReadOnly)
			{
				fileInfo.IsReadOnly = true;
			}
		}
	}
}
