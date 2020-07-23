
namespace RollYourOwnCryptoWPF.ViewModels
{
    class MainViewModel : Observable
    {
        public HashViewModel HashVM { get; private set; }
        public EncDecViewModel EncDecVM { get; private set; }

        public MainViewModel()
        {
            HashVM = new HashViewModel();
            EncDecVM = new EncDecViewModel();
        }
    }
}
