using UnityEngine;
using UnityEngine.UI;
using Lifehack.Spiel.Gui.Stadtplan;

namespace Lifehack.Spiel.Gui {

    public class WechselButton : MonoBehaviour {

        public GameObject spielModulContainer;

        void Start() {
            GetComponent<Button>().onClick.AddListener(this.spielModulContainer.GetComponent<IModul>().SchliesseModul);
        }
    }
}

