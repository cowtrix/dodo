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
using System;
using Dodo.RoleApplications;

namespace DodoAOT
{
	/// <summary>
	/// Is this a dumb way to do things? Maybe. But hey, it works.
	/// I basically just wanted to make something that programatically generated the viewmodels
	/// and view razor pages, because I'm lazy and really it's all just derived from the base 
	/// data model. 
	/// It's just a super simple templating thing that swaps out some magic strings
	/// in the template files (see `DodoCMS\Templates` folder) for their reflected values.
	/// We can probably rip this out once the CMS stuff stabilises a bit if maintainence becomes an issue.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Info($"Generating AOT");
			
			var parsedArgs = new CommandArguments(args);
			var wd = parsedArgs.TryGetValue("workingDir", "");
			if(!string.IsNullOrEmpty(wd))
			{
				Environment.CurrentDirectory = wd;
			}
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
					typeof(Site),
					typeof(RoleApplication)
				})
			{
				OutputReadonlyFile(ViewModelGenerator.Generate(rmType), Path.Combine(viewModelPath, $"{rmType.Name}.cs"));
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

			{
				var folderPath = Path.Combine(viewPath, nameof(RoleApplication));
				OutputReadonlyFile(RoleApplicationViewGenerator.Generate(), Path.Combine(folderPath, $"ViewApplication.cshtml"));
			}
		}

		/// <summary>
		/// Output a readonly text file to the given path
		/// </summary>
		/// <param name="fileContents">The contents of the file</param>
		/// <param name="path">The path to write to</param>
		static void OutputReadonlyFile(string fileContents, string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists && fileInfo.IsReadOnly)
			{
				fileInfo.IsReadOnly = false;
			}
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			using (var fs = new StreamWriter(path, false))
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
