using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Symbols
{
    [CreateAssetMenu(menuName = "SlotMachine/Symbols/new symbols map")]
    public class SymbolsMap : ScriptableObject
    {
        public List<SymbolData> symbols;
        
        private Dictionary<SymbolType, SymbolData> _map;
        private Dictionary<SymbolType, SymbolData> _symbolsMap
        {
            get
            {
                if (_map != null) return _map;
                    _map = new Dictionary<SymbolType, SymbolData>();
                    if (symbols == null || symbols.Count == 0) return _map;
                    symbols.ForEach(symbol => _map.Add(symbol.type, symbol));
                    return _map;
            }
        }
        
  
        private bool _isLoaded;
        private Action _callback;
        public void ReleaseReferences()
        {
            _isLoaded = false;
            _callback = null;
            _map = null;
        }
        
        public bool HasSprite(SymbolType inType)
        {
            if (_symbolsMap.Count == 0 || !_symbolsMap.ContainsKey(inType)) return false;
            return _symbolsMap[inType].sprite != null;
        }
        
        public SymbolData GetData(SymbolType inType)
        {
            if (_symbolsMap.Count == 0) return null;
            return _symbolsMap[inType];
        }
        
        public void LoadAllSymbols(Action callback)
        {
            if(_isLoaded) return;
            _isLoaded = true;
            _callback = callback;
            LoadSprite(0);
        }

       private void LoadSprite(int index)
        {
            void Next()
            {
                index++;
                LoadSprite(index);
            }
            
            if (index >= symbols.Count)
            {
                //Load sprites completed !
                _callback?.Invoke();
            }
            else
            {
                  Next();
            }
        }
    }
}