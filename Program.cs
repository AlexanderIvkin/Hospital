using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> lastNames = new List<string>
            {
                "Кукуев", "Печкин", "Вертляев", "Сидоренко", "Потапенко", "Заслонов", "Креольский", "Чубров", "Семочкин"
            };
            List<string> firstNames = new List<string>
            {
                "Александр", "Виктор", "Алексей", "Сергей", "Дмитрий", "Олег", "Андрей", "Ибрагим", "Варфаломей"
            };
            List<string> patronymics = new List<string>
            {
                "Батькович", "Сергеевич", "Александрович", "Исаакович", "Вессарионович", "Олегович", "Алексеевич"
            };
            List<string> diseasesKinds = new List<string>
            {
                "спидококи", "гонорифилис", "туберкузема", "трихомонилёз", "гиперипахондризм", "стафилоцефалия"
            };
            int[] ageLimits = new int[]
            {
                18, 55
            };
            int patientsCount = 50;
            PatientFactory patientFactory = new PatientFactory(lastNames, firstNames, patronymics, diseasesKinds, ageLimits);
            Hospital hospital = new Hospital(patientFactory, patientsCount);

            hospital.Execute();
        }
    }

    class Hospital
    {
        private const string CommandFullNameSorting = "1";
        private const string CommandAgeSorting = "2";

        private List<Patient> _patients;
        private PatientFactory _patientFactory;

        public Hospital(PatientFactory patientFactory, int patientsCount)
        {
            _patientFactory = patientFactory;
            _patients = _patientFactory.Create(patientsCount);
        }

        public void Execute()
        {
            do
            {
                Console.Clear();
                ShowInfo(_patients);
                ShowMenu();
                ShowInfo(SortPatients(UserUtills.GetUserInput()));
            }
            while (IsRestart());
        }

        private void ShowMenu()
        {
            Console.WriteLine($"\nВыше для ознакомления привёден список всех больных." +
                $"\n\nВведите цифру {CommandFullNameSorting} для сортировки по ФиО." +
                $"\nВведите цифру {CommandAgeSorting} для сортировки по возрасту." +
                $"\nВведите заболевание(капс не важен) для показа заболевших им.\n");
        }

        private List<Patient> SortPatients(string userInput)
        {
            List<Patient> sortedPatients = new List<Patient>();

            switch (userInput)
            {
                case CommandFullNameSorting:
                    sortedPatients = OrederByFullName();
                    break;

                case CommandAgeSorting:
                    sortedPatients = OrderByAge();
                    break;

                default:
                    if (TrySelectByDisease(userInput, out List<Patient> selectedPatients))
                    {
                        sortedPatients = selectedPatients;
                    }
                    break;
            }

            return sortedPatients;
        }

        private List<Patient> OrederByFullName()
        {
            return _patients.OrderBy(patient => patient.FullName).ToList();
        }

        private List<Patient> OrderByAge()
        {
            return _patients.OrderBy(patient => patient.Age).ToList();
        }

        private bool TrySelectByDisease(string userInput, out List<Patient> selectedPatients)
        {
            selectedPatients = _patients.Where(patient => patient.DiseaseKind.ToLower() == userInput.ToLower()).ToList();

            return selectedPatients.Count > 0;
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для повтора.");

            return Console.ReadKey(true).Key != exitKey;
        }

        private void ShowInfo(List<Patient> patients)
        {
            int count = 1;

            if (patients.Count > 0)
            {
                foreach (Patient patient in patients)
                {
                    Console.Write(count++ + " ");
                    patient.ShowInfo();
                }
            }
            else
            {
                Console.WriteLine("А некого показывать. Что-то ввели неверно.");
            }
        }
    }

    class PatientFactory
    {
        private List<string> _lastNames;
        private List<string> _firstNames;
        private List<string> _patronymics;
        private List<string> _diseasesKinds;
        private int[] _ageLimits;

        public PatientFactory(List<string> lastNames, List<string> firstNames, List<string> patronymics, List<string> diseasesKinds, int[] ageLimits)
        {
            _lastNames = lastNames;
            _firstNames = firstNames;
            _patronymics = patronymics;
            _diseasesKinds = diseasesKinds;
            _ageLimits = ageLimits;
        }

        public List<Patient> Create(int count)
        {
            List<Patient> patients = new List<Patient>();

            for (int i = 0; i < count; i++)
            {
                patients.Add(new Patient(GenerateFullName(),
                    _diseasesKinds[UserUtills.GenerateLimitedPositiveNumber(_diseasesKinds.Count)],
                    UserUtills.GenerateNumberFromArrayLimits(_ageLimits)));
            }

            return patients;
        }

        private string GenerateFullName()
        {
            string separator = " ";

            string fullName = _lastNames[UserUtills.GenerateLimitedPositiveNumber(_lastNames.Count)]
                + separator + _firstNames[UserUtills.GenerateLimitedPositiveNumber(_firstNames.Count)]
                + separator + _patronymics[UserUtills.GenerateLimitedPositiveNumber(_patronymics.Count)];

            return fullName;
        }
    }

    class Patient
    {
        public Patient(string fullName, string diseaseKind, int age)
        {
            FullName = fullName;
            DiseaseKind = diseaseKind;
            Age = age;
        }

        public string FullName { get; }
        public string DiseaseKind { get; }
        public int Age { get; }

        public void ShowInfo()
        {
            Console.WriteLine($"{FullName}. Возраст {Age}. Заболевание - {DiseaseKind}.");
        }
    }

    static class UserUtills
    {
        private static Random s_random = new Random();

        public static string GetUserInput()
        {
            Console.Write("Ваш выбор: ");

            return Console.ReadLine();
        }

        public static int GenerateLimitedPositiveNumber(int maxValueExclusive)
        {
            return s_random.Next(maxValueExclusive);
        }

        public static int GenerateNumberFromArrayLimits(int[] limits)
        {
            Array.Sort(limits);

            return s_random.Next(limits[0], limits[1]);
        }
    }
}
