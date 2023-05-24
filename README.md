# Компилятор
## Задание 
Задача №5 к третьей главе учебника:
Пусть грамматика G1[<число>] определена продукциями P:
<число> → <знак> 0 | <знак> 1 | <часть> . | <число> 0 | <число> 1
<часть> → <знак> 0 | <знак> 1 | <часть> 0 | <часть> 1
<знак> → ε | + | –
Построить конечный автомат и преобразовать его к детерминированному, написать алгоритм анализа строк с помощью детерминированного КА.
Примечание: чтение символов автомат производит справа налево

1)	Спроектировать граф конечного автомата
2)	Выполнить программную реализацию алгоритма работы конечного автомата.
Встроить разработанную программу в интерфейс текстового редактора, созданного на первой лабораторной работе.
Пункт меню «Справка» должен содержать полное описание конечного автомата (множество состояний, входной алфавит, начальное состояние, функции переходов в табличном виде и виде графа) и тестовые примеры.
Если задание подразумевает преобразование недетерминированного автомата к детерминированному и/или минимизацию – необходимо описать оба конечных автомата.
## НКА
Описание НКА:
N = (S, Σ, I, δ, F)
S = {S0, S1, S2, S3}
Σ = {0, 1, +, -, .}
I = {S0} – начальное состояние КА
F = {S3} – конечное состояние КА
Таблица переходов для НКА:
| Состояние | Входной символ | Новое состояние |
| --------- | -------------- | --------------- |
| s0 | 0\|1 | s0 |
| s0 | 0\|1 | s1 |
| s0 | . | s2 |
| s1 | +\|-\|ε | s3 |
| s2 | 0\|1 | s2 | 
| s2 | 0\|1 | s1 |

## ДКА
Описание ДКА:
D = (S, Σ, δ, s0, F)
S = {q0, q1, q2, q3, q4}
Σ = {0, 1, +, -, .}
I = {q0} – начальное состояние КА
F = {q1, q3, q4} – конечные состояния КА

Таблица переходов для ДКА:
| Состояние | Входной символ | Новое состояние |
| --------- | -------------- | --------------- |
| q0 | 0\|1 | q1 |
| q0 | . | q2 |
| q1 | 0\|1 | q1 |
| q1 | . | q2 |
| q1 | +\|- | q3 | 
| q2 | 0\|1 | q4 |
| q4 | 0\|1 | q4 |
| q4 | +\|- | q3 | 