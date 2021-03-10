def math_size(Bytes):
    Bytes = float(Bytes)
    KB = float(1024)
    MB = float(KB ** 2)
    GB = float(KB ** 3)
    TB = float(KB ** 4)

    if Bytes < KB:
        return '{0} {1}'.format(Bytes,'Bytes' if 0 == Bytes > 1 else 'Byte')
    elif KB <= Bytes < MB:
        return '{0:.2f} KB'.format(Bytes/KB)
    elif MB <= Bytes < GB:
        return '{0:.2f} MB'.format(Bytes/MB)
    elif GB <= Bytes < TB:
        return '{0:.2f} GB'.format(Bytes/GB)
    elif TB <= Bytes:
        return '{0:.2f} TB'.format(Bytes/TB)