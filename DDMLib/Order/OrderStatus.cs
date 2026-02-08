namespace DDMLib.Order
{
    public enum OrderStatus
    {
        Pending,        // Ожидает обработки
        Processing,     // В обработке
        Assembled,      // Собран
        Shipped,        // Отправлен
        Delivered,      // Доставлен
        Cancelled       // Отменен
    }
}
