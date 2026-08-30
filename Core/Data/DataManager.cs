using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot; 

namespace KingdomCore.Data
{
    public class DataManager
    {
        private Dictionary<string, UnitData> _unitDatabase = new Dictionary<string, UnitData>();

        public void LoadUnitData(string jsonFilePath)
        {
            try
            {
                // Godot's FileAccess est utilisé ici pour supporter la lecture depuis 
                // le système de fichiers virtuel (res://) et les futurs DLC (.pck).
                using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    GD.PrintErr($"Failed to load data file: {jsonFilePath}");
                    return;
                }

                string jsonString = file.GetAsText();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var units = JsonSerializer.Deserialize<List<UnitData>>(jsonString, options);

                if (units != null)
                {
                    _unitDatabase.Clear();
                    foreach (var unit in units)
                    {
                        _unitDatabase[unit.Id] = unit;
                    }
                    GD.Print($"Successfully loaded {units.Count} units from {jsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Exception while parsing JSON {jsonFilePath}: {ex.Message}");
            }
        }

        public UnitData GetUnit(string id)
        {
            if (_unitDatabase.TryGetValue(id, out var data))
            {
                return data;
            }
            
            GD.PrintErr($"Unit with ID '{id}' not found in DataManager.");
            return null;
        }
    }
}
