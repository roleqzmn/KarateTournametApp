using System.Collections.ObjectModel;
using System.Linq;

namespace KarateTournamentApp.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {
        private readonly IndividualCompetitionManagerViewModel _competitionManager;

        public string CategoryName => _competitionManager.Category.Name;
        public ObservableCollection<ParticipantResult> Rankings { get; set; }

        public ResultsViewModel(IndividualCompetitionManagerViewModel competitionManager)
        {
            _competitionManager = competitionManager;
            
            Rankings = _competitionManager.GetFinalRankings();
        }
    }
}
