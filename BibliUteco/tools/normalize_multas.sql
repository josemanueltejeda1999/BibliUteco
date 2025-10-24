BEGIN TRAN;

-- Normalizar a 'Pendiente'
UPDATE Multas
SET Estado = 'Pendiente'
WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado, '')))) = 'pendiente';

-- Normalizar a 'Pagada'
UPDATE Multas
SET Estado = 'Pagada'
WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado, '')))) = 'pagada';

-- Opcional: si quieres forzar NULLs vacíos a 'Pendiente' (comentar si no procede)
UPDATE Multas
SET Estado = 'Pendiente'
WHERE Estado IS NULL OR LTRIM(RTRIM(Estado)) = '';

COMMIT;BEGIN TRAN;

-- Normalizar a 'Pendiente'
UPDATE Multas
SET Estado = 'Pendiente'
WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado, '')))) = 'pendiente';

-- Normalizar a 'Pagada'
UPDATE Multas
SET Estado = 'Pagada'
WHERE LOWER(RTRIM(LTRIM(ISNULL(Estado, '')))) = 'pagada';

-- Opcional: si quieres forzar NULLs vacíos a 'Pendiente' (comentar si no procede)
UPDATE Multas
SET Estado = 'Pendiente'
WHERE Estado IS NULL OR LTRIM(RTRIM(Estado)) = '';

COMMIT;