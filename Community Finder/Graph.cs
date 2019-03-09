using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Community_Finder
{
    class Graph
    {
        public int N, m, Cindex, help;// help used for order  in orionted graphs 
        public bool orionted, Overlap = false;
        public double Q;
        public List<Cluster> Working_clusters;
        public List<Cluster> Finall_clusters;
        public List<Cluster> components;
        public List<Cluster> A;
        public string path="";
        public Graph()//...........................................................................................................................................
        {
            A = new List<Cluster>();
            Working_clusters = new List<Cluster>();
            components = new List<Cluster>();
            Finall_clusters = new List<Cluster>();
        }
        public void Preparing_graph()
        {
            FileInfo graph = new FileInfo(path);
            StreamReader sr = graph.OpenText();
            string s = sr.ReadLine();
            var stringvar = s.Split(';');
            N = stringvar.Length;
            if (stringvar[N - 1] != "0" && stringvar[N - 1] != "1")
                N = N - 1;
            sr.Close();
            sr = graph.OpenText();
            for (int i = 0; i < N; i++)
            {
                s = sr.ReadLine();
                while (s.Length == 0)
                    s = sr.ReadLine();
                for (int f = 0; f < s.Length; f++)
                    stringvar = s.Split(';');
                A.Add(new Cluster());
                for (int j = 0; j < stringvar.Length; j++)
                {
                    if (stringvar[j].CompareTo("1") == 0)
                    {
                        A[i].c.Add(j);
                    }
                }
            }
            sr.Close();
            m = 0;
            for (int i = 0; i < N; i++)
            {
                m = m + A[i].c.Count;
                for (int j = 0; j < A[i].c.Count; j++)
                {
                    if (A[A[i].c[j]].c.Contains(i) == false)
                        orionted = true;
                }
            }
            if (orionted == false)
                m = m / 2;
        }
        public int K_calculation(int I, int in_out = 0)//..........................................................................................................................
        {
            int count = 0;
            if (orionted == false)
            {
                return A[I].c.Count;
            }
            else
            {
                if (in_out == 0)
                {
                    return A[I].c.Count;
                }
                else
                {
                    for (int i = 0; i < N; i++)
                    {
                        if (A[i].c.Contains(I))
                            count++;
                    }
                }
            }
            return count;
        }
        public double P_calculation(int i, int j)//.................................................................................................................
        {
            double P;
            if (orionted == false)
            {
                P = (double)K_calculation(i) * (double)K_calculation(j) / (double)(2 * m);
            }
            else
            {
                P = (double)K_calculation(i, 1) * (double)K_calculation(j) / (double)m;
            }

            return P;
        }
        public byte Same_cluster(int I, int J, int type)//....................................................................................................................
        {
            if (I == J)
                return 1;
            if (type == 1)
            {
                foreach (Cluster cls in Finall_clusters)
                {
                    foreach (int sommet in cls.c)
                    {
                        if (sommet == I)
                        {
                            for (int i = cls.c.IndexOf(sommet) + 1; i < cls.c.Count; i++)
                                if (cls.c[i] == J)
                                {
                                    return 1;
                                }
                            return 0;
                        }
                        else if (sommet == J)
                        {
                            for (int i = cls.c.IndexOf(sommet) + 1; i < cls.c.Count; i++)
                                if (cls.c[i] == I)
                                {
                                    return 1;
                                }
                            return 0;
                        }
                    }

                }
                return 0;
            }
            else
            {
                foreach (Cluster cls in Working_clusters)
                {
                    foreach (int sommet in cls.c)
                    {
                        if (sommet == I)
                        {
                            for (int i = cls.c.IndexOf(sommet) + 1; i < cls.c.Count; i++)
                                if (cls.c[i] == J)
                                {
                                    return 1;
                                }
                            return 0;
                        }
                        else if (sommet == J)
                        {
                            for (int i = cls.c.IndexOf(sommet) + 1; i < cls.c.Count; i++)
                                if (cls.c[i] == I)
                                {
                                    return 1;
                                }
                            return 0;
                        }
                    }
                }
                return 0;
            }
        }
        public double Q_calculation(int type = 0)//............................................................................................................................
        {
            double Q = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Q = Q + ((Convert.ToDouble(A[i].c.Contains(j)) - P_calculation(i, j)) * Same_cluster(i, j, type));
                    //Console.WriteLine(P_calculation(i, j));
                }
            }
            if (orionted == false)
            {
                Q = Q / (double)(2 * m);
            }
            else
            {
                Q = Q / (double)m;
            }
            return Q;
        }
        public bool Exist_in_other_cluster(int ind_cls, int I, int cas)//................................................................................................................
        {
            for (int i = Cindex; i >= 0; i--)
            {
                if (i != ind_cls)
                {
                    if (Working_clusters[i].c.Contains(I))
                        return true;
                    if (Inside_cluster(i, I, cas) == true && Look_for_sommet(I, i) == -1)
                    {
                        Working_clusters[i].c.Add(I);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Delete_if_empty()//.......................................................................................................................
        {
            for (int cls = 0; cls < Working_clusters.Count; cls++)
            {
                if (Working_clusters[cls].c.Count == 0)
                {
                    Working_clusters.RemoveAt(cls);
                    Cindex--;
                }
            }

        }
        public int Look_for_sommet(int I, int ind_C = -1)//.......................................................................................................................
        {
            if (ind_C == -1)
            {
                foreach (Cluster cls in Working_clusters)
                {
                    if (cls.c.Contains(I))
                        return Working_clusters.IndexOf(cls);
                }
                return -1;
            }
            foreach (Cluster cls in Working_clusters)
            {
                if (cls.c.Contains(I) && Working_clusters.IndexOf(cls) != ind_C)
                    return Working_clusters.IndexOf(cls);
            }
            return -1;
        }
        public bool Inside_cluster(int cls, int I, int cas)//................................................................................................................
        {
            int deg = 0, deg_i = 0;
            for (int i = 0; i < A[I].c.Count; i++)
            {
                deg++;
                if (I == A[I].c[i])
                    deg_i++;
                if (Working_clusters[cls].c.Contains(A[I].c[i]) == true)
                {
                    deg_i++;
                    if (A[I].c[i] == I)
                        deg_i++;
                }
            }
            if (orionted == true)
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < A[i].c.Count; j++)
                    {
                        if (A[i].c[j] == I)
                        {
                            deg++;
                            if (I == i)
                                deg_i++;
                            if (Working_clusters[cls].c.Contains(i) == true)
                            {
                                deg_i++;
                                if (i == I)
                                    deg_i++;
                            }
                        }

                    }
                }
            }
            if (cas == 0)
            {
                if (deg <= (Working_clusters[cls].c.Count))
                {
                    if ((float)deg_i / (float)deg >= 0.5)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if ((float)deg_i / (float)(Working_clusters[cls].c.Count) >= 0.5)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                if (deg <= (Working_clusters[cls].c.Count))
                {
                    if ((float)deg_i / (float)deg > 0.5)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if ((float)deg_i / (float)(Working_clusters[cls].c.Count) > 0.5)
                        return true;
                    else
                        return false;
                }
            }

        }
        public void Add_neighbours_to_cluster(int cls, int I, int cas)//....................................................................................................
        {
            for (int i = 0; i < A[I].c.Count; i++)
            {
                if (orionted == true)
                {
                    for (int j = help; j < A[I].c[i]; j++)
                    {
                        if (A[j].c.Contains(I) && Working_clusters[cls].c.Contains(j) == false && Exist_in_other_cluster(cls, j, cas) == false)
                        {
                            if (Inside_cluster(cls, j, cas) == true)
                            {
                                Working_clusters[cls].c.Add(j);
                                Add_neighbours_to_cluster(cls, j, cas);
                            }
                            else
                            {
                                Cindex++;
                                New_cluster(j, cas);
                            }
                        }

                    }
                }
                if (Working_clusters[cls].c.Contains(A[I].c[i]) == false && Exist_in_other_cluster(cls, A[I].c[i], cas) == false)
                {
                    if (Inside_cluster(cls, A[I].c[i], cas) == true)
                    {
                        Working_clusters[cls].c.Add(A[I].c[i]);
                        Add_neighbours_to_cluster(cls, A[I].c[i], cas);
                    }
                    else
                    {
                        Cindex++;
                        New_cluster(A[I].c[i], cas);
                    }
                }
                help = Working_clusters[cls].c[Working_clusters[cls].c.Count - 1];

            }
        }
        public void New_cluster(int I, int cas)//............................................................................................................................
        {
            Working_clusters.Add(new Cluster());
            Working_clusters[Cindex].c.Add(I);
            Add_neighbours_to_cluster(Cindex, I, cas);
        }
        public void Check_equal_bonds(int comp)//.......................................................................................................................
        {
            int ind_cls, location;
            var bonds = new List<int>();
            for (int I = 0; I < components[comp].c.Count; I++)
            {
                int sommet = components[comp].c[I];
                for (int i = 0; i < Working_clusters.Count; i++)
                    bonds.Add(0);
                location = Look_for_sommet(sommet);
                if (location != -1)
                    Working_clusters[location].c.Remove(sommet);
                for (int i = 0; i < A[sommet].c.Count; i++)
                {
                    for (int j = 0; j < Working_clusters.Count; j++)
                    {
                        if (Working_clusters[j].c.Contains(A[sommet].c[i]))
                            bonds[j]++;
                    }
                }
                if (orionted == true)
                {
                    for (int i = 0; i < N; i++)
                    {
                        if (A[i].c.Contains(sommet))
                        {
                            for (int j = 0; j < Working_clusters.Count; j++)
                            {
                                if (Working_clusters[j].c.Contains(i))
                                    bonds[j]++;
                            }
                        }
                    }
                }
                int max = bonds[0], ind_max = 0;
                for (int i = 0; i < bonds.Count; i++)
                {
                    if (bonds[i] > max)
                    {
                        max = bonds[i];
                        ind_max = i;
                    }
                }
                int count = 0;
                for (int i = 0; i < bonds.Count; i++)
                {
                    if (bonds[i] == max)
                        count++;
                }
                if (count > 1)
                {
                    if (Overlap == true)
                    {
                        for (int i = 0; i < bonds.Count; i++)
                        {
                            if (bonds[i] == max)
                            {
                                Working_clusters[i].c.Add(components[comp].c[I]);
                            }
                        }
                    }
                    else
                    {
                        double Q_max = 0, testing;
                        int ind_Q_max = -1;
                        for (int i = 0; i < bonds.Count; i++)
                        {
                            if (bonds[i] == max)
                            {
                                Working_clusters[i].c.Add(components[comp].c[I]);
                                testing = Q_calculation();
                                Working_clusters[i].c.Remove(components[comp].c[I]);
                                if (testing > Q_max)
                                {
                                    Q_max = testing;
                                    ind_Q_max = i;
                                }
                            }
                        }
                        Working_clusters[ind_Q_max].c.Add(components[comp].c[I]);
                    }
                }
                else
                {
                    /*double testing2 = 0, testing1;
                    Working_clusters[ind_max].c.Add(components[comp].c[I]);
                    testing2 = Q_calculation();
                    Working_clusters[ind_max].c.Remove(components[comp].c[I]);
                    Working_clusters[location].c.Add(components[comp].c[I]);
                    testing1 = Q_calculation();
                    if (testing2 > testing1)
                    {
                        Working_clusters[ind_max].c.Add(components[comp].c[I]);
                        Working_clusters[location].c.Remove(components[comp].c[I]);

                    }*/
                    Working_clusters[ind_max].c.Add(components[comp].c[I]);

                }
                bonds.Clear();
            }
        }
        public void Best_distribution(int comp)//............................................................................................................................
        {
            double best_quality = 0, testing;
            int ind_best_quality = 0, best_case = 0;
            for (int cas = 0; cas < 2; cas++)
            {
                for (int i = 0; i < components[comp].c.Count; i++)
                {
                    New_cluster(components[comp].c[i], cas);
                    //Check_equal_bonds();
                    testing = Q_calculation();
                    Working_clusters.Clear();
                    Cindex = 0; help = 0;
                    if (testing > best_quality)
                    {
                        best_quality = testing;
                        ind_best_quality = components[comp].c[i];
                        best_case = cas;
                    }
                }
            }
            New_cluster(ind_best_quality, best_case);
            Check_equal_bonds(comp);
            for (int j = 0; j < Working_clusters.Count; j++)
                Delete_if_empty();
            for (int i = 0; i < Working_clusters.Count; i++)
            {
                Finall_clusters.Add(new Cluster());
                Finall_clusters[Finall_clusters.Count - 1].c.AddRange(Working_clusters[i].c);
            }
            Working_clusters.Clear();
            Cindex = 0;
        }
        public void Identify_component_elements(int comp, int I, ref int counter)//............................................................................................................................
        {
            components[comp].c.Add(I);
            counter++;
            for (int i = 0; i < N; i++)
            {
                if (A[I].c.Contains(i) && components[comp].c.Contains(i) == false)
                    Identify_component_elements(comp, i, ref counter);
                if (orionted)
                {
                    for (int j = 0; j < N; j++)
                        if (A[j].c.Contains(I) && components[comp].c.Contains(j) == false)
                            Identify_component_elements(comp, j, ref counter);
                }
            }
            for (int i = 0; i < components.Count; i++)
                components[i].c.Sort();
        }
        public bool Check_for(int I)//............................................................................................................................
        {
            foreach (Cluster comp in components)
                if (comp.c.Contains(I))
                    return true;
            return false;
        }
        public void Get_connected_components()//............................................................................................................................
        {
            int counter = 0;
            for (int i = 0; i < N; i++)
            {
                if (Check_for(i) == false || components.Count == 0)
                {
                    components.Add(new Cluster());
                    Identify_component_elements(components.Count - 1, i, ref counter);
                }
            }
            for (int i = 0; i < components.Count; i++)
                Best_distribution(i);
            Q = Q_calculation(1);
        }
        public int checknode_inclusters(int I)
        {
            int count=0;
            for(int i=0;i<Finall_clusters.Count;i++)
                if (Finall_clusters[i].c.Contains(I))
                    count++;
            return count;
        }
        public void Generate_a_dot_script()//............................................................................................................................
        {
            var file = new FileInfo(@"pic_generator");
            if (file.Exists)
                file.Delete();
            StreamWriter sw = file.CreateText();
            if (orionted == false)
                sw.WriteLine("strict graph {");
            else
                sw.WriteLine("digraph {");
            sw.WriteLine("    graph[outputorder = edgesfirst];");
            sw.WriteLine("    node [ color=chocolate fixedsize=true shape=circle style=filled width=0.5];");
            sw.WriteLine("    rankdir=\"LR\"");

            var colors = new FileInfo(@"..\..\..\colors.txt");
            StreamReader sr = colors.OpenText();
            for (int i = 0; i < Finall_clusters.Count; i++)
            {
                sw.WriteLine("    subgraph cluster" + i.ToString() + " {");
                sw.WriteLine("        graph [penwidth=1]");
                sw.WriteLine("        node [ fillcolor=" + sr.ReadLine() + "]");
                for (int j = 0; j < Finall_clusters[i].c.Count; j++)
                    if (orionted == false)
                        sw.WriteLine("        " + Finall_clusters[i].c[j] + "-- {} ;");
                    else
                        sw.WriteLine("        " + Finall_clusters[i].c[j] + "-> {} ;");
                sw.WriteLine("    }");
            }
            for (int i = 0; i < A.Count; i++)
            {
                for (int j = 0; j < A[i].c.Count; j++)
                {
                    if (orionted == false)
                        if(Same_cluster(i, A[i].c[j],1)==0 || (Same_cluster(i, A[i].c[j], 1) == 1 && (checknode_inclusters(i) > 1 || checknode_inclusters(A[i].c[j]) >1 )))
                            sw.WriteLine("    " + i + "--" + A[i].c[j] + " [len=3] ;");
                        else
                            sw.WriteLine("    " + i + "--" + A[i].c[j] + " [len=1] ;");
                    else
                        if (Same_cluster(i, A[i].c[j], 1) == 0 || (Same_cluster(i, A[i].c[j], 1) == 1 && (checknode_inclusters(i) > 1 || checknode_inclusters(A[i].c[j]) > 1)))
                        sw.WriteLine("    " + i + "->" + A[i].c[j] + " [len=3] ;");
                    else
                        sw.WriteLine("    " + i + "->" + A[i].c[j] + " [len=1] ;");

                }
            }
            sw.WriteLine("}");
            sw.Close();
            /*string startupPath = System.IO.Directory.GetCurrentDirectory();
            Process DProcess = new Process();
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.StartInfo = info;
            p.Start();

            using (StreamWriter SW = p.StandardInput)
            {
                if (SW.BaseStream.CanWrite)
                {
                    SW.WriteLine("cd" + startupPath);
                    SW.WriteLine("neato -Tpng pic_generator >pic_generator.PNG");
                }
            }*/
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Process pr = new System.Diagnostics.Process();
            pr.StartInfo.WorkingDirectory = startupPath;
            pr.StartInfo.RedirectStandardOutput = true;
            pr.StartInfo.RedirectStandardError = true;
            pr.StartInfo.UseShellExecute = false;

            pr.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            pr.StartInfo.FileName = "neato";
            pr.StartInfo.Arguments = "-Tpng pic_generator";
            pr.Start();

            StreamWriter st = new StreamWriter("pic_generator.png");
            pr.StandardOutput.BaseStream.CopyTo(st.BaseStream);

            pr.StandardOutput.BaseStream.Close();
            st.Close();

        }
        /*public string String_make(int ind_cls, int ind_sommet, int cas = 0)//............................................................................................................................
        {
            int h = Finall_clusters[ind_cls].c[ind_sommet];
            string str;
            int count = 0;
            if (orionted == false)
                str = h.ToString() + " -- { ";
            else
                str = h.ToString() + " -> { ";
            for (int i = 0; i < A[h].c.Count; i++)
            {
                if (cas == 0)
                {
                    if (Finall_clusters[ind_cls].c.Contains(A[h].c[i]))
                    {
                        str = str + A[Finall_clusters[ind_cls].c[ind_sommet]].c[i].ToString() + " ";
                        count++;
                    }
                }
                else
                {
                    if (Finall_clusters[ind_cls].c.Contains(A[h].c[i]) == false && Look_for_sommet(h) != -1)
                    {
                        str = str + A[Finall_clusters[ind_cls].c[ind_sommet]].c[i].ToString() + " ";
                        count++;
                    }
                }
            }
            if (count == 0)
                return "";
            if (cas == 0)
                str = str + "} [len=1]";
            else
                str = str + "} [len=3]";
            return str;
        }*/
    }
}










