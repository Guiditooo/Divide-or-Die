using System.Collections.Generic;
using System;


public class ObjectPool<T>
{
    private Queue<T> _pool; // Cola para almacenar los objetos inactivos
    private HashSet<T> _activeObjects; // Conjunto para almacenar los objetos activos
    private Func<T> _createFunc; // Función para crear nuevos objetos
    private Action<T> _onGet; // Acción al obtener un objeto del pool
    private Action<T> _onRelease; // Acción al devolver un objeto al pool

    public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 10)
    {
        _pool = new Queue<T>(initialSize);
        _activeObjects = new HashSet<T>();
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _onGet = onGet;
        _onRelease = onRelease;

        // Pre-cargar el pool con objetos iniciales
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Enqueue(_createFunc());
        }
    }

    public T Get()
    {
        T obj;
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else
        {
            obj = _createFunc(); // Crear un nuevo objeto si el pool está vacío
        }

        // Registrar el objeto como activo
        _activeObjects.Add(obj);

        // Ejecutar la acción OnGet
        _onGet?.Invoke(obj);

        return obj;
    }

    public void Release(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // Verificar si el objeto está activo
        if (!_activeObjects.Contains(obj))
        {
            return;
        }

        // Ejecutar la acción OnRelease
        _onRelease?.Invoke(obj);

        // Marcar el objeto como inactivo
        _activeObjects.Remove(obj);

        // Devolver el objeto al pool
        _pool.Enqueue(obj);


    }

    /// <summary>
    /// Devuelve todos los objetos activos al pool, reiniciando su estado y desactivándolos si es necesario.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var obj in _activeObjects)
        {
            if (obj != null)
            {
                _onRelease?.Invoke(obj);
                _pool.Enqueue(obj);
            }
        }
        _activeObjects.Clear();
    }
}