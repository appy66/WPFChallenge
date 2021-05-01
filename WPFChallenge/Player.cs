using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFChallenge
{
    public enum Outcome
    {
        Left,
        Center,
        Right,
        Dot
    }

    public class Pot : BindableBase
    {
        private static HashSet<Pot> _validPots = new HashSet<Pot>();
        const int startwithChips = 3;

        public Pot(int playerId)
        {
            PlayerId = playerId;
            Chips = playerId == -1 ? 0 : startwithChips;
        }
        private int _Chips;
        public int Chips
        {
            get { return _Chips; }
            private set 
            {
                SetProperty(ref _Chips, value);
                if (PlayerId == -1) // CenterPot
                    return;
                if (value == 0)
                    _validPots.Remove(this);
                else
                    if (!_validPots.Contains(this))
                        _validPots.Add(this);
            }
        }

        public int PlayerId { get; private set; }

        public static IEnumerable<Pot> GetValids() => _validPots.ToList<Pot>();
        public static void Reset() =>_validPots = new HashSet<Pot>();
        public void Move(Pot target)
        {
            this.Chips--;
            target.Chips++;
        }
        public int Tries => Chips >= startwithChips ? startwithChips : Chips;
    }

    public class Player : BindableBase
    {
        static Random _random = new Random();
        public int Id { get; private set; }

        public Player(int id)
        {   
            Id = id;
            SelfPot = new Pot(Id);
        }

        private Pot _SelfPot;

        public Pot SelfPot
        {
            get { return _SelfPot; }
            private set { SetProperty(ref _SelfPot, value); }
        }

        public bool Play(Pot left, Pot center, Pot right)
        {
            var tries = SelfPot.Tries;
            if (tries == 0)
                return false;
            for (int z = 0; z < tries; z++)
            {
                var o = (Outcome)_random.Next(4);
                switch (o)
                {
                    case Outcome.Center:
                        SelfPot.Move(center);
                        break;
                    case Outcome.Left:
                        SelfPot.Move(left);
                        break;
                    case Outcome.Right:
                        SelfPot.Move(right);
                        break;
                    case Outcome.Dot:
                        break;
                    default:
                        throw new InvalidOperationException("Invalid outcome!");
                }
            }
            return true;
        }
        public bool HasChips() => SelfPot.Chips > 0;
    }
}
