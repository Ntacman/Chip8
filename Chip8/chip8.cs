using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chip8
{
    public class Chip8
    {
        public byte[] memory;
        public byte[] registers;
        public ushort program_counter;
        public byte stack_pointer;
        public ushort[] stack;
        public ushort i;
        public byte[] display_memory;
        public bool running;

        public Chip8()
        {
            running = false;
            memory = new byte[4096];
            display_memory = new byte[2048];
            registers = new byte[16];
            program_counter = 0x200;
            stack_pointer = 0;
            stack = new ushort[16];
            i = 0;
        }

        public Chip8(string rom)
        {
            running = false;
            memory = new byte[4096];
            display_memory = new byte[2048];
            registers = new byte[16];
            program_counter = 0x200;
            stack_pointer = 0;
            stack = new ushort[16];
            i = 0;
            load_rom(rom);
        }

        public void load_rom(string path)
        {
            byte[] contents = File.ReadAllBytes(path);

            for (int i = 0; i < contents.Length; i++)
            {
                var memory_index = program_counter + i;
                memory[memory_index] = contents[i];
            }
        }

        public byte[] Return_memory()
        {
            return memory;
        }

        public byte[] Return_registers()
        {
            return registers;
        }

        public void Set_Register(int register, byte value)
        {
            registers[register] = value;
            Console.WriteLine("Set Register " + register.ToString() + " To " + value.ToString());
        }

        public void Set_Program_Counter(ushort instruction)
        {
            program_counter = instruction;
            Console.WriteLine("Set Program Counter to" + instruction.ToString());
        }

        public void Set_I(byte[] to_be_combined)
        {
            ushort value = BitConverter.ToUInt16(to_be_combined, 0);
            i = value;
            Console.WriteLine("Set I to " + value.ToString());
        }

        public void Skip_If_Equals(int register, byte value)
        {
            if (registers[register] == value)
            {
                program_counter += 2;
                Console.WriteLine("Register " + register.ToString() + "is equal to " + value.ToString());
            }
        }

        public ushort Return_Program_Counter()
        {
            return program_counter;
        }

        public byte[] Return_Display()
        {
            return display_memory;
        }

        public void Process_Instructions(byte[] instructions)
        {
            var op = instructions[0] >> 4;
            switch(op.ToString("X"))
            {
                case "A":
                    {
                        byte[] values = new byte[2];
                        values[0] = (byte)(instructions[0] & 0x0F);
                        values[1] = instructions[1];
                        Set_I(values);
                        program_counter += 2;
                        break;
                    }
                case "1":
                    {
                        ushort instruction = (ushort)((instructions[0] << 8) | instructions[1]);
                        Set_Program_Counter((ushort)(instruction & 0x0FFF));
                        break;
                    }
                case "3":
                    {
                        var register = instructions[0] & 0x0F;
                        Skip_If_Equals(register, instructions[1]);
                        break;
                    }
                case "6":
                    {
                        var register = instructions[0] & 0x0F;
                        Set_Register(register, instructions[1]);
                        program_counter += 2;
                        break;
                    }
                case "0":
                    {
                        if ((instructions[1] >> 4).ToString() != "E" )
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Other 0 Opcodes not handled");
                        }
                        break;
                    }
                default:
                    {
                        System.Console.WriteLine("OP " + op.ToString("X") + " not handled");
                        program_counter += 2;
                        break;
                    }
            }
        }

        public void Run_Once()
        {
            var step = program_counter;
            byte[] temp_instructions = new byte[2];
            temp_instructions[0] = memory[step];
            temp_instructions[1] = memory[step + 1];
            Process_Instructions(temp_instructions);
        }

        public void Run()
        {
            while (running == true)
            {
                var step = program_counter;
                byte[] temp_instructions = new byte[2];
                temp_instructions[0] = memory[step];
                temp_instructions[1] = memory[step + 1];
                Process_Instructions(temp_instructions);
            }
        }
    }

}
