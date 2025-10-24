-- 1) Ver todas las multas (más recientes primero)
SELECT MultaId, PrestamoId, Estado, LEN(ISNULL(Estado,'')) AS EstadoLen, Monto, FechaGenerada, MetodoPago, FechaPago
FROM Multas
ORDER BY FechaGenerada DESC;

-- 2) Contar por valor exacto de Estado (detecta diferencias de mayúsculas/espacios)
SELECT Estado, COUNT(*) AS Cnt
FROM Multas
GROUP BY Estado
ORDER BY Cnt DESC;

-- 3) Mostrar filas con estados que parecen "pendiente" pero con errores (espacios o case)
SELECT MultaId, PrestamoId, Estado, RTRIM(LTRIM(Estado)) AS EstadoTrimmed
FROM Multas
WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado,'')))) = 'pendiente';

-- 4) Comprobar integridad relación Prestamo -> Estudiante y Libro
SELECT m.MultaId, m.PrestamoId, p.EstudianteId, e.NombreCompleto AS Estudiante, p.LibroId, l.Titulo AS Libro
FROM Multas m
LEFT JOIN Prestamos p ON p.PrestamoId = m.PrestamoId
LEFT JOIN Estudiantes e ON e.EstudianteId = p.EstudianteId
LEFT JOIN Libros l ON l.LibroId = p.LibroId
ORDER BY m.FechaGenerada DESC;

-- 5) (Opcional, aplicar SOLO si confirmas) Normalizar estados 'pendiente' con espacios / case
-- BEGIN TRAN;
-- UPDATE Multas
-- SET Estado = 'Pendiente'
-- WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado,'')))) = 'pendiente';
-- COMMIT;																																																													