using System.Linq;
using System.IO;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.WorkingGroups;
using Dodo.Roles;
using Dodo.LocationResources;
using Dodo;
using Common.Commands;

namespace DodoAOT
{

	class Program
	{
		static void Main(string[] args)
		{
			var parsedArgs = new CommandArguments(args);
			var viewModelPath = Path.GetFullPath(parsedArgs.MustGetValue<string>("viewmodels"));
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
				using (var fs = new StreamWriter(Path.Combine(folderPath, $"Create.cshtml")))
				{
					fs.Write(CreateViewGenerator.Generate(rmType));
				}
				using (var fs = new StreamWriter(Path.Combine(folderPath, $"Edit.cshtml")))
				{
					fs.Write(EditViewGenerator.Generate(rmType));
				}
				using (var fs = new StreamWriter(Path.Combine(folderPath, $"Delete.cshtml")))
				{
					fs.Write(DeleteViewGenerator.Generate(rmType));
				}
			}
		}
	}
}
