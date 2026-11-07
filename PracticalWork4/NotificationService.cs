namespace PracticalWork4
{
    public class NotificationService
    {
        public IEnumerable<Notification> FilterAndSort(
            IEnumerable<Notification> notifications,
            NotificationFilterOptions options)
        {
            if (notifications == null) return Enumerable.Empty<Notification>();
            if (options == null) return notifications;

            var query = notifications.AsQueryable();

            if (options.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == options.IsRead.Value);
            }

            if (options.Types != null && options.Types.Length > 0)
            {
                query = query.Where(n => options.Types.Contains(n.Type));
            }

            if (!string.IsNullOrWhiteSpace(options.SearchText))
            {
                var text = options.SearchText.Trim().ToLower();
                query = query.Where(n =>
                    (n.Title != null && n.Title.ToLower().Contains(text)) ||
                    (n.Content != null && n.Content.ToLower().Contains(text))
                );
            }

            if (options.MinPriority.HasValue)
            {
                query = query.Where(n => n.Priority >= options.MinPriority.Value);
            }

            if (options.SortBy.HasValue)
            {
                query = options.SortBy.Value switch
                {
                    SortNotificationBy.Date => options.Descending
                        ? query.OrderByDescending(n => n.CreatedAt)
                        : query.OrderBy(n => n.CreatedAt),

                    SortNotificationBy.Priority => options.Descending
                        ? query.OrderByDescending(n => n.Priority)
                        : query.OrderBy(n => n.Priority),

                    SortNotificationBy.Title => options.Descending
                        ? query.OrderByDescending(n => n.Title)
                        : query.OrderBy(n => n.Title),

                    _ => query
                };
            }

            return query.ToList();
        }
    }
}