using WebAppFakeUsers.Models;
using Bogus;



namespace WebAppFakeUsers.Services
{
    public class FakeUserDataService
    {
        private readonly Dictionary<string, Faker> _regionFakers = new()
        {
            {"USA", new Faker("en")},
            {"Poland", new Faker("pl")},
            {"Uzbekistan", new Faker("en")}
        };

		public List<FakeUser> GenerateFakeUsers(int userCount, int pageNumber, string seedInput, double errorRate, string region)
		{
			int seed = seedInput.GetHashCode() + pageNumber;
			Random random = new Random(seed);

		
			Console.WriteLine("pageNumber " + pageNumber + " seed " + seed + " Random " + random);

			var users = new List<FakeUser>();

			
			Faker faker;
			if (_regionFakers.ContainsKey(region))
			{
				faker = _regionFakers[region];
				faker.Random = new Bogus.Randomizer(seed); 
			}
			else
			{
				faker = new Faker() { Random = new Bogus.Randomizer(seed) }; 
			}

			for (var i = 0; i < userCount; i++)
			{
				var userIndex = i + 1 + (pageNumber - 1) * userCount;
				var fakeUser = new FakeUser
				{
					Number = userIndex,
					Identifier = GeneratePseudoGuid(random),
					FullName = faker.Name.FullName(),
					Address = GenerateAddress(faker),
					PhoneNumber = faker.Phone.PhoneNumber()
				};

				if (region == "Uzbekistan")
				{
					fakeUser.FullName = GenerateUzbekFullName(random);
					fakeUser.Address = GenerateUzbekAddress(random);
					fakeUser.PhoneNumber = GenerateUzbekPhoneNumber(random);
				}

				IntroduceErrors(fakeUser, errorRate, random);
				users.Add(fakeUser);
			}

			return users;
		}


		private string GenerateUzbekAddress(Random random)
        {
            var streets = new[] { "Mustaqillik", "Amir Temur", "Navoi", "Islom Karimov", "Buyuk Ipak Yuli" };
            var cities = new[] { "Toshkent", "Samarqand", "Buxoro", "Farg'ona", "Namangan" };

            var street = streets[random.Next(streets.Length)];
            var houseNumber = random.Next(1, 100);
            var city = cities[random.Next(cities.Length)];

            return $"Street {street}, House {houseNumber}, {city}";
        }

		private string GenerateUzbekPhoneNumber(Random random)
		{
			var phonePrefixes = new[] { "90", "91", "93", "94", "95", "97", "98", "99" };
			var prefix = phonePrefixes[random.Next(phonePrefixes.Length)];
			var number = random.Next(1000000, 9999999).ToString();

			return $"+998 {prefix} {number}";
		}

		private string GenerateUzbekFullName(Random random)
		{
			var firstNames = new[] { "Aziz", "Gulnara", "Nodir", "Dilshod", "Munira" };
			var lastNames = new[] { "Khamraev", "Sobirova", "Yusupov", "Abdullaev", "Karimov" };

			return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
		}
		private string GeneratePseudoGuid(Random random)
		{
			byte[] buffer = new byte[16];
			random.NextBytes(buffer);
			return new Guid(buffer).ToString();
		}

		public static string GenerateAddress(Faker faker)
        {
			return faker.Address.FullAddress();
		}

		public void IntroduceErrors(FakeUser user, double errorRate, Random random)
		{
			double errorPercent = errorRate / 1000.0;

			if (random.NextDouble() < errorRate / 1000.0 && user.FullName != null)
			{
				int times = (int)(user.FullName.Length * errorPercent);  
				for (int i = 0; i < times; i++)
				{
					user.FullName = IntroduceError(user.FullName, random);
				}
			}
			if (random.NextDouble() < errorRate / 1000.0 && user.Address != null)
			{
				int times = (int)(user.Address.Length * errorPercent);
				for (int i = 0; i < times; i++)
				{
					user.Address = IntroduceError(user.Address, random);
				}
			}
			if (random.NextDouble() < errorRate / 1000.0 && user.PhoneNumber != null)
			{
				int times = (int)(user.PhoneNumber.Length * errorPercent);
				for (int i = 0; i < times; i++)
				{
					user.PhoneNumber = IntroduceError(user.PhoneNumber, random);
				}
			}
		}
		private string IntroduceError(string text, Random random)
		{
			int operation = random.Next(4);  

			switch (operation)
			{
				case 0:
					int removePos = random.Next(text.Length);
					text = text.Remove(removePos, 1);
					break;
				case 1:
					int addPos = random.Next(text.Length + 1);
					char addChar = (char)('a' + random.Next(26));
					text = text.Insert(addPos, addChar.ToString());
					break;
				case 2:
					if (text.Length > 1)
					{
						int swapPos = random.Next(text.Length - 1);
						char temp = text[swapPos];
						text = text.Substring(0, swapPos) + text[swapPos + 1] + temp + text.Substring(swapPos + 2);
					}
					break;
				case 3: 
					if (text.Length > 0)
					{
						int replacePos = random.Next(text.Length);
						char replaceChar = (char)('a' + random.Next(26));
						text = text.Substring(0, replacePos) + replaceChar + text.Substring(replacePos + 1);
					}
					break;
			}

			return text;
		}


	}
}
