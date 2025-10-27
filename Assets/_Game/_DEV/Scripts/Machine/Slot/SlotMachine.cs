using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2Int = Helper.Vector2Int;

namespace SlotGame.Machine
{
    public class SlotMachine : BaseMonoBehaviour
    {
        [Header("Internal References")]
        [SerializeField] private MachineConfig config;
        [SerializeField] private RectTransform slotsHolder;
        [SerializeField] private SpinButton spinBtn;
        [SerializeField] private ViewFades viewFades;
        [SerializeField] private ViewTweener viewTweener;
        [SerializeField] private Slot slotPrefab;
        
        //-events
        public event Action onEstablished;
        public event Action<int, List<string>> OnSpinCompleted;
        
        //- private variables
        private List<Slot> _slots;
        private int _size;
        private Vector2Int _dimension;
        private bool _isAutoSpin;
        private bool _canSpin;
        private List<Column> _columns;
        
        //- properties
        public Slot[,] Matrix { get; private set; }
        public MachineConfig Config => config;
        protected override void ReleaseReferences()
        {
            viewTweener = null;
            viewFades = null;
            spinBtn = null;
            slotsHolder = null;
            config = null;
            slotPrefab = null;
            _slots = null;
            Matrix = null;
            _columns = null;
        }
        
        public void Initialize()
        {
            spinBtn.onClick.AddListener(Spin);
            spinBtn.onHoldClick.AddListener(HoldSpin);
            spinBtn.IsInteractable = false;
            _isAutoSpin = spinBtn.IsPressed;
            viewFades.HideImmediately();
            viewTweener.CloseImmediately();
        }

        [ContextMenu("Show")]
        public void Show()
        {
            viewFades.FadeIn();
            viewTweener.Open();
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            viewFades.FadeOut();
            viewTweener.Close();
        }
        
        private IEnumerator WaitAndServe(float inDelay, Action inService)
        {
            yield return new WaitForSeconds(inDelay);
            inService?.Invoke();
        }
        
        private void HoldSpin(bool inStatus)
        {
            _isAutoSpin = inStatus;
            if(_isAutoSpin) Spin();
            else StopSpinningOneShot();
        }

        private void Spin()
        {
            if (!_canSpin)
            {
                if (config.forceStopSpin) StopSpinningOneShot();
                return;
            }
            
            switch (Config.startSpinning)
            {
                case SpinningType.OneShot : StartSpinningOneShot(); break;
                case SpinningType.Individually : StartSpinningIndividually(Config.individuallyDelay); break;
            }
        }

        private void StartSpinningOneShot()
        {
            _canSpin = false;
            spinBtn.UpdateShape(true);
            foreach (var column in _columns)
                column.Spin();
            StartCountingSpinningDuration();
        }

        private void StartSpinningIndividually(float inDelay)
        {
            _canSpin = false;
            spinBtn.UpdateShape(true);
            SpinColumn(0, inDelay);
        }

        private void SpinColumn(int index, float inDelay)
        {
            if (index >= _columns.Count)
            {
                StartCountingSpinningDuration();
                return;
            }
            
            _columns[index].Spin();
            StartCoroutine(WaitAndServe(inDelay, () =>
            {
                index++;
                SpinColumn(index, inDelay);
            }));
        }

        private void StartCountingSpinningDuration()
        {
            var duration = Random.Range(Config.spinningDurationRange.x, Config.spinningDurationRange.y);
            StartCoroutine(WaitAndServe(duration, StopSpinning));
        }
        
        private void StopSpinning()
        {
            switch (Config.endSpinning)
            {
                case SpinningType.OneShot : StopSpinningOneShot(); break;
                case SpinningType.Individually : StopSpinningIndividually(Config.individuallyDelay); break;
            }
        }

        private void StopSpinningOneShot()
        {
            StopAllCoroutines();
            spinBtn.UpdateShape(false);
            _canSpin = true;
            foreach (var column in _columns)
                column.Stop(SpinComplete);
        }

        private void StopSpinningIndividually(float inDelay)
        {
            StopAllCoroutines();
            StopColumn(0, inDelay);
        }

        private void StopColumn(int index, float inDelay)
        {
            if (index >= _columns.Count)
            {
                spinBtn.UpdateShape(false);
                _canSpin = true;
                return;
            }
            
            var isLastColumn = index == _columns.Count - 1;
            if (isLastColumn)
            {
                _columns[index].Stop(SpinComplete);
            }
            else
            {
                _columns[index].Stop(null);
            }
            StartCoroutine(WaitAndServe(inDelay, () =>
            {
                index++;
                StopColumn(index, inDelay);
            }));
        }
        
        private void SpinComplete()
        {
            _canSpin = true;

            // Evaluate result
            SlotScoreEvaluator evaluator = new SlotScoreEvaluator(this);
            int totalScore = evaluator.EvaluateScore();

            // Collect visible symbols for JSON log
            List<string> symbols = new List<string>();
            for (int x = 0; x < _dimension.x; x++)
            {
                for (int y = 0; y < _dimension.y; y++)
                {
                    symbols.Add(Matrix[x, y].CurrentSymbol.ToString());
                }
            }

            // Trigger callback
            OnSpinCompleted?.Invoke(totalScore, symbols);

            if (_isAutoSpin)
                StartCoroutine(WaitAndServe(Config.autoSpinDelay, Spin));
        }

        private void ClearMachine()
        {
            if (_slots == null)
            {
                _slots = new List<Slot>();
                return;
            }

            foreach (var slot in _slots)
                slot.RemoveSlot();
            
            _slots.Clear();
        }
        
        public void GenerateSlots(Vector2Int inDimension)
        {
            _dimension = inDimension;
            
            ClearMachine();
            
            _size = _dimension.x * _dimension.y;
            SpawnSlot(0);
         
        }
        
           private void SpawnSlot(int index)
          {
              if (index >= _size) CheckGenerationCompleted();
              else
              {
                  void Next()
                  {
                      index++;
                      SpawnSlot(index);
                  }
                  GameObject slot = Instantiate(slotPrefab.gameObject);
                  slot.GetComponent<Slot>().rectTransform.SetParent(slotsHolder);
                  _slots.Add(slot.GetComponent<Slot>());
                  Next();
              }
          }

         private void CheckGenerationCompleted()
         {
             if (_slots.Count != _size) throw new Exception($"slots List size must be equals the slot machine size {_size} ");
             var slotSize = _slots[0]?.rectTransform.sizeDelta;
             if (!slotSize.HasValue) throw new Exception($"Slot size doesn't have a value");
             
             Matrix = new Slot[_dimension.x, _dimension.y];
             for (var x = 0; x < _dimension.x; x++)
             {
                 for (var y = 0; y < _dimension.y; y++)
                 {
                     //- define
                     var location = new Vector2Int(x, y);
                     var targetSlot =  _slots[x * _dimension.y + y];
                     
                     //- assign
                      targetSlot.Initialize(this, location);
                     Matrix[x, y] = targetSlot;
                 }
             }
             AssignColumns();
         }
         
        private void AssignColumns()
        {
            _columns = new List<Column>();
            for (var y = 0; y < _dimension.y; y++)
            {
                var column = new Column($"Col [{y}]", slotsHolder, this);
                for (var x = 0; x < _dimension.x; x++)
                    column.AttachSlot(Matrix[x,y]);
                _columns.Add(column);
            }
            AddFakeRow();
        }
        
        private void AddFakeRow()
        {
                for (var i = 0; i < _dimension.y; i++)
                {
                    var slot = Instantiate(slotPrefab, slotsHolder).GetComponent<Slot>();
                    slot.Initialize(this, new Vector2Int(-1, i));
                    _columns[i].AttachFakeSlot(slot, true);
                    
                    var slot2 = Instantiate(slotPrefab, slotsHolder).GetComponent<Slot>();
                    slot2.Initialize(this, new Vector2Int(-2, i));
                    _columns[i].AttachFakeSlot(slot2, false);
                }
                SlotMachineEstablished();
        }

        private void SlotMachineEstablished()
        {
            spinBtn.IsInteractable = true;
            _canSpin = true;
            Show();
            spinBtn.GetComponent<ViewFades>().FadeIn();
            onEstablished?.Invoke();
        }

        public void Tick(float deltaTime)
            => _columns.ForEach(column => { column.Tick(deltaTime); });

    }
}