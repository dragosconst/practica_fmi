import numpy as np
import math
import os
import matplotlib.pyplot as pyplot

# ------------------------------------------------------------
# logging functions
# ------------------------------------------------------------

def log_population(pop, domain, prec, coefs, file):
    for index in range(pop.shape[0]):
        file.write(str(index + 1) +": ")
        file.write(str(pop[index]))
        file.write(" x= ")
        file.write(str(translate(pop[index], domain, prec)))
        file.write(" f= ")
        file.write(str(fitness(pop[index], domain, prec, coefs)))
        file.write('\n')
        file.flush()

def log_init_pop(pop, gen, domain, prec, coefs, f):
    f.write("Populatia generatia " + str(gen) + "\n")
    f.flush()
    log_population(pop, domain, prec, coefs, f)
    f.flush()


def log_elitist(crom, index, domain, prec, coefs, f):
    f.write("\n")
    f.write("Cromozomul selectat elitist este cel cu indicele: " + str(index))
    f.write(", cu x= " + str(translate(crom, domain, prec)) + " si f= " + str(fitness(crom, domain, prec, coefs)))
    f.write('\n')
    f.flush()


def log_probs(pop_fit, f):
    f.write('\n')
    f.write("Probabilitati selectie\n")
    f.flush()
    for index in range(pop_fit.shape[0]):
        f.write("cromozom")
        for ws in range(4 - len(str(index + 1))):
            f.write(" ")
        f.write(str(index + 1))
        f.write(" probabilitate ")
        f.write(str(pop_fit[index] / np.sum(pop_fit)))
        f.write('\n')
        f.flush()
    f.flush()


def log_intervs(pop_interv, f):
    f.write("Intervale de probabilitati de selectie\n")
    f.write("0 ")
    for dr in pop_interv:
        f.write(str(dr) + " ")
    f.write('\n')
    f.flush()


def log_unif_interv(u, index, f):
    f.write("u= " + str(u))
    f.write(" selectam cromozomul " + str(index))
    f.write('\n')
    f.flush()

def log_after_selection(newpop, domain, prec, coefs, f):
    f.write("Dupa selectie:\n")
    f.flush()
    log_population(newpop, domain, prec, coefs, f)
    f.write('\n')
    f.flush()


def log_cross_text(cross, f):
    f.write("Probabilitatea de incrucisare " + str(cross) + "\n")
    f.flush()


def log_cross_prob(x, u, index, cross, f):
    f.write(str(index) + ": ")
    f.write(str(x) + " u=" + str(u))
    if u < cross:
        f.write(" <" + str(cross) + " participa")
    f.write('\n')
    f.flush()


def log_comb(p1, p2, i1, i2, cut, c1, c2, f):
    f.write("\nRecombinare dintre cromozomul " + str(i1) + " cu cromozomul " + str(i2) + ":\n")
    f.write(str(p1) + " " + str(p2) + " punct " + str(cut) + "\n")
    f.write("Rezultat: " + str(c1.astype(int)) + " " + str(c2.astype(int)) + "\n")
    f.flush()

def log_comb3(p1, p2, p3, i1, i2, i3, cut, c1, c2, c3, f):
    f.write("\nRecombinare dintre cromozomul " + str(i1) + " cu cromozomul " + str(i2) + " cu cromozomul " + str(i3) + ":\n")
    f.write(str(p1) + " " + str(p2) + " " + str(p3) + " punct " + str(cut) + "\n")
    f.write("Rezultat: " + str(c1.astype(int)) + " " + str(c2.astype(int)) + " " + str(c3.astype(int)) + "\n")
    f.flush()


def log_after_cross(pop, domain, coefs, prec, f):
    f.write("Dupa recombinare:\n")
    log_population(pop, domain, prec, coefs, f)
    f.write('\n')
    f.flush()


def log_mutation(indexes, mut, f):
    f.write("Probabilitatea de mutatie pentru fiecare cromozom: " + str(mut) + "\n")
    f.write("Au fost modificati cromozomii:\n")
    f.flush()
    for index in indexes:
        f.write(str(index + 1) + "\n")
        f.flush()


def log_after_mut(pop, domain, prec, coefs, f):
    f.write("Dupa mutatie:\n")
    log_population(pop, domain, prec, coefs, f)
    f.write('\n')
    f.flush()


def log_best(chrs, means, domain, prec, coefs, f):
    f.write("Evolutia fitness-ului:\n")
    f.flush()
    gen = 1
    for chr in chrs:
        f.write("Generatia " + str(gen) + ")")
        f.write(" x= " + str(translate(chr, domain, prec)))
        f.write(" f=" + str(fitness(chr, domain, prec, coefs)))
        f.write(" media=" + str(means[gen - 1]))
        gen += 1
        f.write('\n')
        f.flush()
    f.flush()


# ------------------------------------------------------------
# functiile legate de logica algoritmului
# ------------------------------------------------------------

# Cromozomii o sa fie reprezentati ca numpy arrays
# Populatiile sunt arrays de cromozomi


# am folosit reprezentare cu least significant bit first
def translate(x, domain, prec):
    x_10 = x.dot(2**np.arange(x.size))
    coefx = (domain[1] - domain[0]) / (2 ** x.size - 1)
    return round(coefx * x_10 + domain[0], prec)

# Primeste ca argument un cromozom + coeficientii
# E functia f
def fitness(x, domain, prec, coefs):
    x_10 = translate(x, domain, prec)
    xs = np.array([1, x_10, x_10 ** 2])
    return xs.dot(coefs)



# selectie proportionala cu metoda ruletei
def proportional_select(pop, domain, prec, coefs, f):
    # construiesc un array cu intervale
    pop_fitness = np.apply_along_axis(fitness, 1, pop, domain, prec, coefs)
    pop_prop = np.divide(pop_fitness, np.sum(pop_fitness))
    pop_inter = np.zeros(pop_prop.shape)
    log_probs(pop_fitness, f)
    # pop_inter o sa aiba o lista cu capatul din dreapta al intervalelor
    for i in range(pop_prop.shape[0]):
        if i > 0:
            pop_inter[i] = pop_inter[i - 1] + pop_prop[i]
        else:
            pop_inter[i] = pop_prop[i]
    log_intervs(pop_inter, f)

    # metoda ruletei
    # final_pop = np.zeros(pop.shape) - GRESIT! - trebuie sa fie cu 1 mai putin pe prima dimensiune
    final_pop = np.zeros((pop.shape[0] - 1, pop.shape[1]))
    for i in range(pop_prop.shape[0] - 1):
        u = np.random.uniform()
        sel_index = np.searchsorted(pop_inter, u + 1e-9) # ca sa am interval deschis la dreapta
        final_pop[i] = pop[sel_index]
        log_unif_interv(u, sel_index + 1, f)
    return final_pop



# O functie auxiliara care face perechi din parintii dati, pentru crossover
def get_pairs(parents):
    parent_pair = []
    parents_left = parents.shape[0]
    tmp_parents = parents
    # generez perechile de parinti
    while parents_left > 0:
        if parents_left == 3:  # cazul cand era un numar impar de elemente
            parent_pair = parent_pair + [tmp_parents[:]]
            parents_left -= 3
        else:
            parent_pair = parent_pair + [tmp_parents[:2]]
            tmp_parents = np.delete(tmp_parents, [0, 1], axis=0)
            parents_left -= 2

    return parent_pair

# O functie auxiliara care schimba toti parintii din populatie cu copii
def swap_parents(pop, parents, children):
    pr_ch_index = 0 # index comun pt parinti si copii
    for ind in range(pop.shape[0]):
        if (pop[ind] == parents[pr_ch_index]).all(): # pt ca parintii sunt selectati cu un for, stiu sigur ca asta e ordinea in care o sa fie gasiti
            pop[ind] = children[pr_ch_index]
            pr_ch_index += 1
            if pr_ch_index >= len(parents):
                break
    return pop # might be unnecessary ??

# Incrucisare cu un punct de rupere
def crossover(pop, cross, f):
    # construiesc parintii
    parents = []
    log_indexes = []
    i = 1
    for ind in pop:
        u = np.random.uniform()
        if u < cross:
            parents = parents + [ind]
            log_indexes = log_indexes + [i]
        log_cross_prob(ind, u, i, cross, f)
        i += 1
    parents = np.array(parents).astype(int)
    if parents.shape[0] <= 1: # daca am avut ghinion si e selectat un singur individ pt cross over, n avem ce face
        return pop

    # incrucisarile efective
    parent_pair = get_pairs(parents)

    children = np.zeros(parents.shape)
    child_index = 0
    for pair in parent_pair:
        cut = np.random.randint(1, len(pair[0]) - 1)
        children[child_index] = np.concatenate((pair[0][:cut], pair[1][cut:]))
        child_index += 1
        children[child_index] = np.concatenate((pair[1][:cut], pair[0][cut:]))
        child_index += 1
        if len(pair) == 3: # daca avem 3 elemente in pair
            children[child_index - 1] = np.concatenate((pair[1][:cut], pair[2][cut:]))
            children[child_index] = np.concatenate((pair[2][:cut], pair[0][cut:])) # ca sa ma asigur ca am toti parintii fol in taietura
            child_index += 1
            log_comb3(pair[0], pair[1], pair[2], log_indexes[child_index - 3], log_indexes[child_index - 2], log_indexes[child_index - 1],
                      cut, children[child_index - 3], children[child_index - 2], children[child_index - 1], f)
        else:
            log_comb(pair[0], pair[1], log_indexes[child_index - 2], log_indexes[child_index - 1], cut,
                     children[child_index - 2], children[child_index - 1], f)
    return swap_parents(pop, parents, children)


def rare_mutation(pop, mut, file):
    indexes = []
    for i in range(pop.shape[0]):
        u = np.random.uniform()
        if u < mut:
            pos = np.random.randint(0, pop.shape[1])
            pop[i][pos] = (1 - pop[i][pos])
            indexes += [i]
    log_mutation(indexes, mut, file)
    return pop # might be unnecessary x2 ???


#
# Formatul pt argumente:
# - pop_dim: un int
# - domain: un tuple sau o lista, important e ca e de forma [a,b], a < b
# - coefs: un tuple sau o lista, important sa fie ordonati incepand cu coeficientul termenului liber
# - precision: un int care reprezinta puterea lui 10
# - cross: un float care rep probabilitatea
# - mutation: un float care rep probabilitatea
# - generations: un int
# - fName: optional, numele fisierului de iesire
# - max_iter_no_change: optional, cate generatii sa ruleze fara nicio imbunatatire la maxim (default e -1, adica nu se opreste)
def evol_alg(pop_dim, domain, coefs, precision, cross, mutation, generations, fName="Evolutie.txt", max_iter_no_change=-1):
    L = math.ceil(np.log2((domain[1] - domain[0]) * 10 ** precision))
    print(L)
    population = np.random.randint(0, 2, (pop_dim, L)) # folosesc distributia uniforma built in din numpy
    some_plotting = []
    max_generations = generations

    try:
        os.remove(fName)
    except OSError as e:
        print(e.filename, e.strerror)
    file = open(fName, "a")
    best_chrs = []
    means = []

    old_best = -1
    same_generations = 0

    while generations > 0:
        log_init_pop(population, max_generations - generations, domain, precision, coefs, file)
        # Selectie elitista
        best_chr = np.argmax(np.apply_along_axis(fitness, 1, population, domain, precision, coefs))
        log_elitist(population[best_chr], best_chr + 1, domain, precision, coefs, file)
        some_plotting += [fitness(population[best_chr], domain, precision, coefs)]
        best_chrs += [population[best_chr]]

        # Selectie proportionala
        new_inter_pop  = proportional_select(np.copy(population), domain, precision, coefs, file).astype(int)
        log_after_selection(new_inter_pop, domain, precision, coefs, file)

        # Incrucisare
        log_cross_text(cross, file)
        crossed_pop = crossover(np.copy(new_inter_pop), cross, file)
        log_after_cross(crossed_pop, domain, coefs, precision, file)

        # Mutatie
        mutated_pop = rare_mutation(np.copy(crossed_pop), mutation, file)
        log_after_mut(mutated_pop, domain, precision, coefs, file)

        # populatia finala obtinuta
        population = np.append(mutated_pop, [population[best_chr]], axis = 0)
        means += [np.apply_along_axis(fitness, 1, population, domain, precision, coefs).mean()]
        generations -= 1
        print(generations)

        # daca e setat un nr maxim de iteratii fara schimbari la maxim
        if max_iter_no_change > 1:
            if old_best != -1 and old_best == some_plotting[len(some_plotting) - 1]:
                same_generations += 1
            else:
                same_generations = 1
            if same_generations == max_iter_no_change:
                print("A fost depasit numarul maxim de iteratii fara nicio schimbare la maxim")
                break
            old_best = some_plotting[len(some_plotting) - 1]

    fig, (alg, fun, mean) = pyplot.subplots(1, 3, figsize=(20, 5))
    fig.suptitle("Algoritmul si functia si media pe domeniu")
    alg.plot(np.arange(len(some_plotting)), some_plotting, color='green')
    fun.plot(np.arange(domain[0], domain[1], 1e-3),
            np.apply_along_axis((lambda x: coefs[0] + coefs[1] * x + coefs[2] * x **2), 0, np.arange(domain[0], domain[1], 1e-3)), color='red')
    mean.plot(np.arange(len(means)), means, color="blue")
    print(some_plotting)

    pyplot.show()
    log_best(best_chrs, means, domain, precision, coefs, file)
    file.close()
# ------------------------------------------------------------


# evol_alg(50, [900, 1998.829], [-2340, 2000, -1], 6, 0.85, 0.01, 100)
# evol_alg(10, [-3, 3], [2, 0, 1], 6, 0.5, 0.1, 100, max_iter_no_change=30)
evol_alg(20, [-1, 2], [2, 1, -1], 6, 0.25, 0.01, 50)