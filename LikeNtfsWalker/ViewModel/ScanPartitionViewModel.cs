﻿using Filesystem.Partition;
using LikeNtfsWalker.Model;
using LikeNtfsWalker.UI;
using System.Collections.ObjectModel;
using Filesystem.Ntfs;
using System.Collections.Generic;
using System.Windows;
using Util.IO;
using System.Linq.Expressions;
using System;

namespace LikeNtfsWalker.ViewModel
{
    public class ScanPartitionViewModel : Notifier
    {
        private Scan selectparttition;

        public Scan SelectParttition
        {
            get => selectparttition;
            set
            {
                selectparttition = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Scan> ScanList { get; set; }


        public ScanPartitionViewModel(Model.Disk disk)
        {
            ScanList = new ObservableCollection<Scan>();

            try
            {
                Mbr mbr = new Mbr(disk.FilePath);

                DeviceStream stream = new DeviceStream(disk.FilePath, 512);

                if (mbr.signature != 21930) //Signature Value
                {
                    ScanList.Add(new Scan("There is no MBR", ""));
                }
                else
                {
                    foreach (var partition in mbr.partitions)
                    {
                        stream.Position = (long)partition.StartingLBAAddr * 512;
                        VBR vbr = new VBR(stream); //VBR 넘겨줄깜?

                        ScanList.Add(new Scan(vbr.Lable, GetPartitionType(partition.PartitionType)));
                    }

                }
            }
            catch
            {
                MessageBox.Show("파티션 테이블이 MBR이 아닙니다.");
            }
        }

        public string GetPartitionType(int type)
        {
            switch (type)
            {
                case 12:
                    return "FAT32";
                case 7:
                    return "NTFS";

                default:
                    return "";
            }
        }
    }
}
