class Program {
	public static void Main(){
		// Getting all the info from the text files
		var movies = getAllMovies(System.IO.File.ReadAllLines(@".\Products.txt").ToList());
		var users = getUserOrSession(System.IO.File.ReadAllLines(@".\Users.txt").ToList());
		var sessions = getUserOrSession(System.IO.File.ReadAllLines(@".\CurrentUserSession.txt").ToList());
		
		// Task 1 Recommended
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Task 1: Recommended movies based on rating and purchase history \n");
		Console.ResetColor();

		getRecommendation(movies, users, 4.2);
		
		// Task 2 Recommended based on current watch genre
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"\nTask 2: Recommended movies based on users current watch category randomized");
		Console.ResetColor();
		
		users.ForEach(delegate(List<string> user){ 
			getUserRecommendation(movies, sessions, user);
		});
	}

	// Task 2 method
 	public static void getUserRecommendation(List<List<string>> movies, List<List<string>> sessions, List<string> user){
		var tempList = new List<string>();

		sessions.ForEach(delegate(List<string> session){
			if(session[0] == user[0]){
				var sessionMovie = new List<string>();
				var moviesGenreMatch = new List<string>();

				movies.ForEach(delegate(List<string> movie){
					if(session[1].Trim() == movie[0]){
						sessionMovie = movie;
						return;
					}
				});

				var sessionMovieGenres = sessionMovie[3].Split(",");
				movies.ForEach(delegate(List<string> movie){
					var movieGenres = movie[3].Split(",").ToList();
					movieGenres.ForEach(delegate(string genre){
						if(Array.IndexOf(sessionMovieGenres, genre) > -1){
							moviesGenreMatch.Add(movie[1]);
							return;
						}
					});
				});

				IEnumerable<int> sequence = Enumerable.Range(0, moviesGenreMatch.Count-1).OrderBy( n => n * n * ( new Random() ).Next() );
				var results = sequence.Distinct().Take(3).ToList();

				results.ForEach(delegate(int number){
					tempList.Add(moviesGenreMatch[number]);
				});
				return;
			}
		});

		if(tempList.Any()){
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(
				$"\nUserID: {user[0].Trim()}"+
				$"- UserName: {user[1].Trim()}"+
				"- Recommendations"
				);
			Console.ResetColor();
			tempList.ForEach(delegate(string title){
				Console.WriteLine($"\t{title}");
			});
		}

	}

	// Task 1 method
	public static void getRecommendation(List<List<string>> movies, List<List<string>> users, double rating = 0, bool multipleSales = false){
		var tempList = new List<List<string>>();
		var tempSales = new List<int>();
		if(multipleSales){

			users.ForEach(delegate(List<string> user){
				string[] purchased = user[3].Split(";");

				movies.ForEach(delegate(List<string> movie){
					if(Array.IndexOf(purchased, movie[0]) > -1){
						tempSales.Add(Int32.Parse(movie[0]));
						var duplicates = tempSales
							.GroupBy(i => i)
							.Where(g => g.Count() > 1)
							.Select(g => g.Key);
						if(double.Parse(movie[4], System.Globalization.CultureInfo.InvariantCulture) > rating && duplicates.Cast<int>().Contains(Int32.Parse(movie[0]))){
							tempList.Add(movie);
						}else if(rating == 0 && duplicates.Cast<int>().Contains(Int32.Parse(movie[0]))){
							tempList.Add(movie);
						}
					}
				});
			});

		}else if(!multipleSales){
			var movieList = new List<List<string>>();
			movies.ForEach(delegate(List<string> movie){
				if(double.Parse(movie[4], System.Globalization.CultureInfo.InvariantCulture) > rating){
					movieList.Add(movie);
				}
			});

			IEnumerable<int> sequence = Enumerable.Range(0, movieList.Count-1).OrderBy( n => n * n * ( new Random() ).Next() );
			var results = sequence.Distinct().Take(3).ToList();
			results.ForEach(delegate(int number){
					tempList.Add(movieList[number]);
				});
		}

		tempList.ForEach(delegate(List<string> movie){
			Console.WriteLine(
					$"Title: {movie[1]}"+
					$"\n\tYear: {movie[2]}"+
					$"\n\tCategories: {movie[3]}"+
					$"\n\tRating: {movie[4]}"+
					$"\n\tPrice: {movie[5]}\n"
			);
		});
	}

	// Getting info from files (Users.txt and CurrentUserSession.txt)
	public static List<List<string>> getUserOrSession(List<string> list){
		var tempList = new List<List<string>>();

		list.ForEach(delegate(string item){
			tempList.Add(item.Split(",").ToList());
		});
		return tempList;
	}

	// Getting info from file (Products.txt)
	public static List<List<string>> getAllMovies(List<string> list){
		var tempMovies = new List<List<string>>();

		list.ForEach(delegate(string movie){
			var movieInfo = movie.Split(",");
			var tempList = new List<string>();
			string genres = "";

			foreach(var item in movieInfo.Select((value, index) => new {index, value})){
				if(item.index < 3 || item.index > 7){
					tempList.Add(item.value.Trim());
				}else if(item.index == 3){
					genres = item.value.Trim();
				}else if(!string.IsNullOrWhiteSpace(item.value)){
					genres += ","+item.value;
				}
				if(item.index == 7){
					tempList.Add(genres);
				}
			}
			tempMovies.Add(tempList);
		});
		return tempMovies;
	}
}