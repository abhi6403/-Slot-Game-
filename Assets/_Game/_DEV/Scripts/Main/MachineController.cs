using SlotGame.Machine;
using SlotGame.Symbols;
using UnityEngine;
using Vector2Int = Helper.Vector2Int;

namespace SlotGame
{
    public class MachineController : BaseMonoBehaviour
    {
        //- singleton
        private static MachineController _instance;
        public static SymbolsMap SymbolsMap => _instance.symbolsMap;
        
        //- exposed fields
        [SerializeField] private SlotMachine slotMachine;
        [SerializeField] private SymbolsMap symbolsMap;
        [SerializeField] private Vector2Int dimension;
        
        //- private variables
        private bool _isRun;
        
        protected override void ReleaseReferences()
        {
            slotMachine.onEstablished -= Launch;
            symbolsMap.ReleaseReferences();
            symbolsMap = null;
            slotMachine = null;
        }

        private void Awake()
        {
            if(_instance != null) return;
            _instance = this;
        }

        private void Start()
        {
            //- entry point!
            slotMachine.onEstablished += Launch;
            slotMachine.Initialize();
            StartLoadingSymbols();
        }

        private void StartLoadingSymbols()
        {
            symbolsMap.LoadAllSymbols(() => { slotMachine.GenerateSlots(dimension); });
        }

        private void Launch()
        {
            _isRun = true;
        }
        
        private void Update()
        {
            if(!_isRun) return;
            slotMachine.Tick(Time.deltaTime);
        }
    }
}
