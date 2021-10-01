using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 浮点型修饰器
    /// </summary>
    public class FloatModifier
    {
        public float Value;
        public FloatModifier (float value = 0)
        {
            Value = value;
        }
    }

    /// <summary>
    /// 浮点型修饰器集合
    /// </summary>
    public class FloatModifierCollection
    {
        public float TotalValue { get; private set; }
        private List<FloatModifier> Modifiers { get; } = new List<FloatModifier>();

        public float AddModifier(FloatModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return TotalValue;
        }

        public float RemoveModifier(FloatModifier modifier)
        {
            Modifiers.Remove(modifier);
            Update();
            return TotalValue;
        }

        public void Update()
        {
            TotalValue = 0;
            foreach (var item in Modifiers)
            {
                TotalValue += item.Value;
            }
        }
    }
    /// <summary>
    /// 浮点型数值
    /// </summary>
    public class FloatNumeric
    {
        public float Value { get; private set; }
        public float baseValue { get; private set; }
        public float add { get; private set; }
        public float pctAdd { get; private set; }
        public float finalAdd { get; private set; }
        public float finalPctAdd { get; private set; }
        private FloatModifierCollection AddCollection { get; } = new FloatModifierCollection();
        private FloatModifierCollection PctAddCollection { get; } = new FloatModifierCollection();
        private FloatModifierCollection FinalAddCollection { get; } = new FloatModifierCollection();
        private FloatModifierCollection FinalPctAddCollection { get; } = new FloatModifierCollection();

        public FloatNumeric(float value = 0)
        {
            SetBase(value);
        }

        public void Initialize()
        {
            baseValue = add = pctAdd = finalAdd = finalPctAdd = 0f;
        }
        public float SetBase(float value)
        {
            baseValue = value;
            Update();
            return baseValue;
        }

        public FloatModifier AddModifier(AddNumericType addNumericType, float value = 0)
        {
            FloatModifier modifier = new FloatModifier(value);
            switch(addNumericType)
            {
                case AddNumericType.Add:
                    AddAddModifier(modifier);
                    break;
                case AddNumericType.PctAdd:
                    AddPctAddModifier(modifier);
                    break;
                case AddNumericType.FinalAdd:
                    AddFinalAddModifier(modifier);
                    break;
                case AddNumericType.FinalPctAdd:
                    AddFinalPctAddModifier(modifier);
                    break;
                default:
                    Log.Error("get error type when add modifier" + addNumericType.ToString());
                    return null;
            }
            return modifier;
        }
        public void RemoveModifier(AddNumericType addNumericType, FloatModifier modifier)
        {
            switch (addNumericType)
            {
                case AddNumericType.Add:
                    RemoveAddModifier(modifier);
                    break;
                case AddNumericType.PctAdd:
                    RemovePctAddModifier(modifier);
                    break;
                case AddNumericType.FinalAdd:
                    RemoveFinalAddModifier(modifier);
                    break;
                case AddNumericType.FinalPctAdd:
                    RemoveFinalPctAddModifier(modifier);
                    break;
                default:
                    Log.Error("get error type when remove modifier" + addNumericType.ToString());
                    break;
            }
        }

        public void AddAddModifier(FloatModifier modifier)
        {
            add = AddCollection.AddModifier(modifier);
            Update();
        }
        public void AddPctAddModifier(FloatModifier modifier)
        {
            pctAdd = PctAddCollection.AddModifier(modifier);
            Update();
        }
        public void AddFinalAddModifier(FloatModifier modifier)
        {
            finalAdd = FinalAddCollection.AddModifier(modifier);
            Update();
        }
        public void AddFinalPctAddModifier(FloatModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.AddModifier(modifier);
            Update();
        }

        public void RemoveAddModifier(FloatModifier modifier)
        {
            add = AddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(FloatModifier modifier)
        {
            pctAdd = PctAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(FloatModifier modifier)
        {
            finalAdd = FinalAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalPctAddModifier(FloatModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.RemoveModifier(modifier);
            Update();
        }

        public void Update()
        {
            var value1 = baseValue;
            var value2 = (value1 + add) * (100 + pctAdd) / 100f;
            var value3 = (value2 + finalAdd) * (100 + finalPctAdd) / 100f;
            Value = value3;
        }
    }
}