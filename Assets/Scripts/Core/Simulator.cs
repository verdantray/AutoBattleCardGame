using System;
using AutoBattleCardGame.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AutoBattleCardGame.Core
{
    public class Simulator : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameDataAsset>(GameConst.Address.GAME_DATA_ASSET);
                await handle.Task;
                
                Storage.CreateInstance(handle.Result);
                Addressables.Release(handle);
                
                ScriptedPlayer playerA = new ScriptedPlayer("PlayerA");
                ScriptedPlayer playerB = new ScriptedPlayer("PlayerB");
                
                Simulation simulation = new Simulation(playerA, playerB);
                await simulation.RunAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Simulator has error occured : {e}");
                throw; // TODO 예외 처리
            }
            finally
            {
                Application.Quit();
            }
        }
    }
}
