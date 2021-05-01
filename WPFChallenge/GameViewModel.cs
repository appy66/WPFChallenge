using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFChallenge
{
    public class GameViewModel : BindableBase
    {
        public GameViewModel()
        {
            StartCommand = new DelegateCommand(OnStartCommand, CanStartCommand);
            Stats = new ObservableCollection<GameStat>();
        }

        private int _PlayerCount = 3;

        public int PlayerCount
        {
            get { return _PlayerCount; }
            set 
            {
                if (SetProperty(ref _PlayerCount, value))
                    StartCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _GameRunning = false;

        public bool GameRunning
        {
            get { return _GameRunning; }
            set 
            {
                if (SetProperty(ref _GameRunning, value))
                    StartCommand.RaiseCanExecuteChanged();
            }
        }

        private Pot _CenterPot;

        public Pot CenterPot
        {
            get { return _CenterPot; }
            set { SetProperty(ref _CenterPot, value); }
        }

        private int _Iteration;

        public int Iteration
        {
            get { return _Iteration; }
            set { SetProperty(ref _Iteration, value); }
        }

        public DelegateCommand  StartCommand{ get; set; }
        private bool CanStartCommand() => PlayerCount >= 3 && PlayerCount < 1000000 && !GameRunning;

        private ObservableCollection<Player> _Players;

        public ObservableCollection<Player> Players
        {
            get { return _Players; }
            set { SetProperty(ref _Players, value); }
        }

        private void OnStartCommand()
        {
            GameRunning = true;
            Pot.Reset();
            CenterPot = new Pot(-1);
            Players = new ObservableCollection<Player>();
            for (int i = 0; i < PlayerCount; i++)
            {
                var player = new Player(i);
                Players.Add(player);
            }

            var _stat = new GameStat() { Duration = new TimeSpan(), Iterations = 0 };
            Stats.Add(_stat);
            Task.Factory.StartNew(() =>
            {
                var startTime = DateTime.Now;
                var play = true;
                while (play)
                {
                    var index = 0;
                    foreach (var p in Players)
                    {
                        if ( p.Play(
                                index == 0 ? Players.Last().SelfPot : Players[index - 1].SelfPot,
                                CenterPot,
                                index == (PlayerCount - 1) ? Players.First().SelfPot : Players[index + 1].SelfPot) )
                        {
                            NonZeroPlayers = Pot.GetValids().Count();
                            if ( NonZeroPlayers == 1)
                            {
                                play = false;
                                _stat.Winner = Pot.GetValids().First().PlayerId.ToString();
                                break;
                            }
                        }
                        _stat.Iterations++;
                        _stat.Duration = DateTime.Now - startTime;
                        index++;
                    }
                }

                _stat.Duration = DateTime.Now - startTime;

                FastestGame = _Stats.Min(x => x.Duration.TotalMilliseconds);
                AverageGame = _Stats.Average(x => x.Duration.TotalMilliseconds);
                GameRunning = false;
            });
        }

        private ObservableCollection<GameStat> _Stats;

        public ObservableCollection<GameStat> Stats
        {
            get { return _Stats; }
            set { SetProperty(ref _Stats, value); }
        }

        private double _FastestGame;

        public double FastestGame
        {
            get { return _FastestGame; }
            set { SetProperty(ref _FastestGame, value); }
        }

        private double _AverageGame;

        public double AverageGame
        {
            get { return _AverageGame; }
            set { SetProperty(ref _AverageGame, value); }
        }

        private int _NonZeroPlayers;

        public int NonZeroPlayers
        {
            get { return _NonZeroPlayers; }
            set { SetProperty(ref _NonZeroPlayers, value); }
        }


    }

    public class GameStat : BindableBase
    {
        private int _Iterations;

        public int Iterations
        {
            get { return _Iterations; }
            set { SetProperty(ref _Iterations, value); }
        }

        private TimeSpan _Duration;

        public TimeSpan Duration
        {
            get { return _Duration; }
            set { SetProperty(ref _Duration, value); }
        }

        private string _Winner;

        public string Winner
        {
            get { return _Winner; }
            set { SetProperty(ref _Winner, value); }
        }

    }

}
