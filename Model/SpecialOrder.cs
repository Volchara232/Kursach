namespace Sem3_kurs.Model
{
    public class SpecialOrder : Order
    {
        public int PriorityLevel { get; set; } = 1;
        public string IndividualManager { get; set; }
        public string IndividualConditions { get; set; }

        public void SetPriority(int priority)
        {
            if (priority < 1) priority = 1;
            PriorityLevel = priority;
        }

        public string GetFullSpecialInfo()
        {
            return $"Приоритет: {PriorityLevel}, менеджер: {IndividualManager}, условия: {IndividualConditions}";
        }
    }
}


