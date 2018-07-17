namespace Kongverge.Common.Services
{
    public class KongAction
    {
        public static KongAction<T> Success<T>(T result)
        {
            return new KongAction<T>
            {
                Succeeded = true,
                Result = result
            };
        }

        public static KongAction<T> Failure<T>()
        {
            return new KongAction<T> { Succeeded = false };
        }
    }

    public class KongAction<T>
    {
        public T Result { get; internal set; }

        public bool Succeeded { get; internal set;  }
    }
}
