using System;
namespace JokesWebAppTester.Models
{
	public class Joke
	{
		public int  id { get; set; }
		public string JokeQuestion { get; set; }
		public string JokeAnswer { get; set; }

		public Joke()
		{
		}
	}
}

