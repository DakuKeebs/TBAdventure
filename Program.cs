using System;
using System.Collections.Generic;
using System.IO;

namespace TBAdventure
{
	public class Program
	{
		private static int Main(string[] args)
		{
			Game game = new Game();
			game.Play();
			
			return 0;
		}
	}
	
	public static class TextWriter
	{
		public static bool debug_mode = true;
		
		public static void Write(string text, ConsoleColor background_colour, ConsoleColor foreground_colour)
		{
			if (!TextWriter.debug_mode)
			{
				return;
			}
			
			Console.BackgroundColor = background_colour;
			Console.ForegroundColor = foreground_colour;
			Console.Write(text);
			Console.ResetColor();
		}
		
		public static void WriteLine(string text, ConsoleColor background_colour, ConsoleColor foreground_colour)
		{
			if (!TextWriter.debug_mode)
			{
				return;
			}
			
			Console.BackgroundColor = background_colour;
			Console.ForegroundColor = foreground_colour;
			Console.WriteLine(text);
			Console.ResetColor();
		}
		
		public static void WriteDebug(string text)
		{
			TextWriter.WriteLine(text, ConsoleColor.Black, ConsoleColor.Cyan);
		}
		
		public static void WriteError(string text)
		{
			TextWriter.WriteLine(text, ConsoleColor.Black, ConsoleColor.Red);
		}
	}
	
	public class Game
	{
		public bool is_running;
		public List<TextSection> game_data;
		public TextSection current_section;
		
		public Game()
		{
			is_running = false;
			game_data = new List<TextSection>();
			current_section = null;
		}
		
		public void Play()
		{
			is_running = true;
			
			LoadGameData();
			
			// Get the Start section.
			current_section = LoadSection("Start");
			
			Console.WriteLine("\n\n\n");
			while (is_running)
			{
				Console.Clear();
				
				foreach (string line in current_section.text_data.data)
				{
					TextWriter.WriteLine(line, ConsoleColor.Black, ConsoleColor.White);
				}
				
				foreach (string option in current_section.option_data.data)
				{
					TextWriter.WriteLine(option, ConsoleColor.Black, ConsoleColor.Blue);
				}
				
				Console.Write("Enter Command: ");
				string input = Console.ReadLine();
				
				bool found_option = false;
				int index = 0;
				for (int i = 0; i < current_section.option_data.data.Count; ++i)
				{
					if (input == current_section.option_data.data[i])
					{
						Console.WriteLine("FUCK ME SIDEWAYS");
						found_option = true;
						index = i;
						break;
					}
				}
				
				if (found_option)
				{
					string thing = current_section.action_data.data[index].Split(':')[1];
					current_section = LoadSection(thing);
				}
				
				if (input == "quit")
				{
					is_running = false;
				}
			}
			
			Console.Write("Press ENTER to Exit.");
			Console.ReadLine();
		}
		
		public void LoadGameData()
		{
			Loader test_loader = new Loader();
			game_data = test_loader.LoadGameData(new string[]{ "./GameExample.txt" });
		}
		
		public TextSection LoadSection(string section_name)
		{
			foreach (TextSection section in game_data)
			{
				if (section.title == section_name)
				{
					return section;
				}
			}
			
			return new TextSection();
		}
	}
	
	public class TextData
	{
		public List<string> data;
		
		public TextData()
		{
			data = new List<string>();
		}
		
		public void Add(string data_to_add)
		{
			data.Add(data_to_add);
		}
	}
	
	public class OptionData
	{
		public List<string> data;
		
		public OptionData()
		{
			data = new List<string>();
		}
		
		public void Add(string data_to_add)
		{
			data.Add(data_to_add);
		}
	}
	
	public class ActionData
	{
		public List<string> data;
		
		public ActionData()
		{
			data = new List<string>();
		}
		
		public void Add(string data_to_add)
		{
			data.Add(data_to_add);
		}
	}
	
	public class TextSection
	{
		public string title;
		public TextData text_data;
		public OptionData option_data;
		public ActionData action_data;
		
		public TextSection()
		{
			title = String.Empty;
			text_data = new TextData();
			option_data = new OptionData();
			action_data = new ActionData();
		}
	}
	
	public class StoryLine
	{
		public string title;
		public IList<TextSection> text_sections;
		
		public StoryLine(string title, string filePath)
		{
			this.title = title;
			// Load file, story in text_sections.
		}
	}
	
	public class Loader
	{
		public List<TextSection> game_data;
		
		public Loader()
		{
			game_data = new List<TextSection>();
		}
		
		public List<TextSection> LoadGameData(string[] file_paths)
		{
			foreach (string file_path in file_paths)
			{
				LoadGameFile(file_path);
			}
			
			foreach (TextSection text_section in game_data)
			{
				Console.WriteLine(text_section.title);
				
				foreach (string text in text_section.text_data.data)
				{
					Console.WriteLine("- >> {0}", text);
				}
				
				foreach (string text in text_section.option_data.data)
				{
					Console.WriteLine("+ >> {0}", text);
				}
			}
			
			return game_data;
		}
		
		public int LoadGameFile(string file_path)
		{
			TextWriter.WriteDebug(String.Format("Finding file to load - '{0}'", file_path));
			
			game_data.Clear();
			
			if (!File.Exists(file_path))
			{
				TextWriter.WriteError(String.Format("Cannot open files - '{0}'", file_path));
				return -1;
			}
			
			TextWriter.WriteLine(String.Format("Opening file - '{0}'", file_path), ConsoleColor.Black, ConsoleColor.Yellow);
			string[] file_data = File.ReadAllLines(file_path);
			
			// Parse data.
			int i = 0;
			for (; i < file_data.Length; ++i)
			{
				TextWriter.WriteLine(file_data[i], ConsoleColor.Black, ConsoleColor.Magenta);
				
				string current_line = file_data[i];
				
				if (current_line == String.Empty)
				{
					continue;
				}
				
				char line_start_char = current_line[0];
				switch (line_start_char)
				{
					case '!':
						TextWriter.WriteDebug(String.Format("Found character '{0}'", line_start_char));
						
						game_data.Add(new TextSection());
						game_data[game_data.Count - 1].title = current_line.Substring(1, current_line.Length - 1);
						
						break;
					case '-':
						TextWriter.WriteDebug(String.Format("Found character '{0}'", line_start_char));
						
						game_data[game_data.Count - 1].text_data.Add(current_line.Substring(1, current_line.Length - 1));
						
						break;
					case '+':
						TextWriter.WriteDebug(String.Format("Found character '{0}'", line_start_char));
						
						current_line = current_line.Substring(1);
						
						string[] split_string = current_line.Split('|');
						game_data[game_data.Count - 1].option_data.Add(split_string[0]);
						game_data[game_data.Count - 1].action_data.Add(split_string[1]);
						
						break;
					default:
						TextWriter.WriteError(String.Format("Unrecoginsed identifier '{0}'", line_start_char));
						return -1;
				}
			}
			
			TextWriter.WriteLine(String.Format("Closing file - '{0}'", file_path), ConsoleColor.Black, ConsoleColor.Yellow);
			
			return 0;
		}
	}
}




