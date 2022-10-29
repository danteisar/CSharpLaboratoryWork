using Currency;

namespace LaboratoryWork3
{   
    internal static class ProductExtension
    {
        public static void OnCourseChange(this IProduct product, object source, CourseEventArgs args)
        {
            if (product == null) return;
            if ((Rub)args.NewCourse < (Usd)args.NewCourse)
            {
                product.Rub = product.Rub;
            }
            else
            {
                product.Usd = product.Usd;
            }
        }
    }
}
