﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace chucknorrisimportquiz.Migrations
{
    [DbContext(typeof(NorrisContext))]
    partial class NorrisContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("ChuckNorrisJoke", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ChuckNorrisId")
                        .HasMaxLength(40)
                        .HasColumnType("int");

                    b.Property<string>("Joke")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("Id");

                    b.ToTable("chuckNorrisJokes");
                });
#pragma warning restore 612, 618
        }
    }
}
