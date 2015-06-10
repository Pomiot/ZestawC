using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Input;

namespace Cards
{
    /// <summary>
    /// Main class for View Model
    /// TODO: follow guidelines
    /// </summary>
    public class CardsViewModel : ICardsViewModel
    {
        private readonly IDispatcher _dispatcher;
        public CardsViewModel(IDispatcher dispatcher)
        {


            ChosenRanks = new List<CardRank>();
            ChosenSuits = new List<CardSuit>();
            EvaluatedSets = new ObservableCollection<ProbabilitySet>();
            AvailableRanks = Enum.GetValues(typeof (CardRank)).Cast<CardRank>().ToList();
            AvailableSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToList();
            
            _dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<ProbabilitySet> _evaluatedSets;
        public ObservableCollection<ProbabilitySet> EvaluatedSets { get; private set; }
        public IList<CardRank> AvailableRanks { get; private set; }

        private IList<CardRank> _chosenRanks;
        public IList<CardRank> ChosenRanks
        {
            get { return _chosenRanks; }
            set
            {
                this._chosenRanks = value;
                OnPropertyChanged("ChosenRanks");
            }
        }

        public IList<CardSuit> AvailableSuits { get; private set; }
        private IList<CardSuit> _chosenSuits;

        public IList<CardSuit> ChosenSuits
        {
            get
            {
                return this._chosenSuits;
            }
            set
            {
                this._chosenSuits = value;
                OnPropertyChanged("ChosenSuits");
            }
        }

        private ProbabilitySet _highestProbability;
        public ProbabilitySet HighestProbability { get; private set; }

        private ICommand _saveSearchesCommand;

        public ICommand SaveSearchesCommand
        {
            get
            {
                if (_saveSearchesCommand == null)
                {
                    _saveSearchesCommand = new MyCommand(param => this.SaveSearchesCommandExecute(),
                        param => this.SaveSearchesCommandCanExecute);
                }

                return _saveSearchesCommand;
            }
        }

        public bool SaveSearchesCommandCanExecute
        {
            get
            {
                return true;
            }
        }

        private void SaveSearchesCommandExecute()
        {

            if (EvaluatedSets.Count == 0)
            {
                return;
            }
            var setsSerializer = new DataContractJsonSerializer(typeof(List<ProbabilitySet>));

            using (var fs = File.OpenWrite("EvaluatedSets.json"))
            {
                setsSerializer.WriteObject(fs, EvaluatedSets);
            }
        }

        private ICommand _calcCommand;
        public ICommand CalcCommand
        {
            get
            {
                if (_calcCommand == null)
                {
                    _calcCommand = new MyCommand(param => this.CalcCommandExecute(),
                        param => this.CalcCommandCanExecute);
                }
                return _calcCommand;
            }
        }

        public bool CalcCommandCanExecute
        {
            get
            {
                return true;
            }
        }

        private void CalcCommandExecute()
        {

            if (ChosenSuits.Count == 0 && ChosenRanks.Count == 0)
            {
                MessageBox.Show("Nie można obliczyć prawdopodobieństwa.");
                return;
            }

            ProbabilitySet result = new ProbabilitySet
            {
                Ranks = new List<CardRank>(ChosenRanks),
                Suits = new List<CardSuit>(ChosenSuits),
                Probability = ((ChosenSuits.Count*AvailableRanks.Count) + (ChosenRanks.Count*AvailableSuits.Count) -
                               (ChosenSuits.Count*ChosenRanks.Count))/52.0
            };

            this.EvaluatedSets.Add(result);
            OnPropertyChanged("EvaluatedSets");
        }

        private ICommand _showHighestCommand;

        public ICommand ShowHighestCommand
        {
            get
            {
                if (_showHighestCommand == null)
                {
                    _showHighestCommand = new MyCommand(param => ShowHighestCommandExecute(),
                        param => ShowHighestCommandCanExecute);
                }
                return _showHighestCommand;
            }
        }

        public bool ShowHighestCommandCanExecute
        {
            get
            {
                return true;
            }
        }

        private void ShowHighestCommandExecute()
        {
            if (EvaluatedSets.Count > 0)
            {
                ProbabilitySet result = EvaluatedSets.OrderByDescending(item => item.Probability).First(); ;
                HighestProbability = result;
                OnPropertyChanged("HighestProbability");
            }
            
        }
    }
}
