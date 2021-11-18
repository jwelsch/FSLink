using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command.Scan;
using FSLinkCommon.Wraps;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace FSLinkCommand.Tests.Command.Scan
{
    public class FileSystemScannerTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_path_is_file_then_do_one_callback_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = AutoFixture.Create<SearchOption>();
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Normal);

            //var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            //directoryWrap.EnumerateFiles();

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(path);
            callbackCount.Should().Be(1);
        }

        [Fact]
        public void When_path_is_empty_directory_then_do_no_callbacks_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = AutoFixture.Create<SearchOption>();
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(Array.Empty<string>());
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(Array.Empty<string>());

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(path);
            callbackCount.Should().Be(0);
        }

        [Fact]
        public void When_path_is_directory_with_files_then_do_callbacks_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var fileCount = 3;
            var filePaths = AutoFixture.CreateMany<string>(fileCount);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = AutoFixture.Create<SearchOption>();
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(filePaths);
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(Array.Empty<string>());

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(path);
            callbackCount.Should().Be(fileCount);
        }

        [Fact]
        public void When_path_is_directory_with_directories_then_do_callbacks_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var directoryCount = 3;
            var directoryPaths = AutoFixture.CreateMany<string>(directoryCount);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = AutoFixture.Create<SearchOption>();
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(Array.Empty<string>());
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(directoryPaths);

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(path);
            callbackCount.Should().Be(directoryCount);
        }

        [Fact]
        public void When_path_is_directory_with_directories_and_files_then_do_callbacks_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var directoryCount = 2;
            var fileCount1 = 2;
            var fileCount2 = 2;
            var directoryPaths = AutoFixture.CreateMany<string>(directoryCount);
            var filePaths1 = AutoFixture.CreateMany<string>(fileCount1);
            var filePaths2 = AutoFixture.CreateMany<string>(fileCount2);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = SearchOption.AllDirectories;
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(filePaths1, filePaths2);
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(directoryPaths);

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1 + directoryCount).GetAttributes(Arg.Any<string>());
            callbackCount.Should().Be(directoryCount + fileCount1 + fileCount2);
        }

        [Fact]
        public void When_callback_for_file_returns_false_then_stop_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var directoryCount = 2;
            var fileCount1 = 2;
            var fileCount2 = 2;
            var directoryPaths = AutoFixture.CreateMany<string>(directoryCount);
            var filePaths1 = AutoFixture.CreateMany<string>(fileCount1);
            var filePaths2 = AutoFixture.CreateMany<string>(fileCount2);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = SearchOption.AllDirectories;
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return callbackCount < 2;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(filePaths1, filePaths2);
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(directoryPaths);

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(Arg.Any<string>());
            callbackCount.Should().Be(2);
        }

        [Fact]
        public void When_callback_for_directory_returns_false_then_stop_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var directoryCount = 2;
            var fileCount1 = 2;
            var fileCount2 = 2;
            var directoryPaths = AutoFixture.CreateMany<string>(directoryCount);
            var filePaths1 = AutoFixture.CreateMany<string>(fileCount1);
            var filePaths2 = AutoFixture.CreateMany<string>(fileCount2);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = SearchOption.AllDirectories;
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return callbackCount < 3;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(filePaths1, filePaths2);
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(directoryPaths);

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(Arg.Any<string>());
            callbackCount.Should().Be(3);
        }

        [Fact]
        public void When_path_is_directory_with_directories_and_files_and_searchoption_is_topdirectoryonly_then_only_do_first_level_and_return()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var directoryCount = 2;
            var fileCount1 = 2;
            var fileCount2 = 2;
            var directoryPaths = AutoFixture.CreateMany<string>(directoryCount);
            var filePaths1 = AutoFixture.CreateMany<string>(fileCount1);
            var filePaths2 = AutoFixture.CreateMany<string>(fileCount2);
            var searchPattern = AutoFixture.Create<string>();
            var searchOption = SearchOption.TopDirectoryOnly;
            var callbackCount = 0;
            Func<string, bool> callback = p =>
            {
                callbackCount++;
                return true;
            };

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(path).Returns(FileAttributes.Directory);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.EnumerateFiles(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(filePaths1, filePaths2);
            directoryWrap.EnumerateDirectories(path, searchPattern, Arg.Any<EnumerationOptions>()).Returns(directoryPaths);

            var sut = autoSub.Resolve<FileSystemScanner>();

            sut.ScanPath(path, searchPattern, searchOption, callback);

            fileWrap.Received(1).GetAttributes(path);
            callbackCount.Should().Be(directoryCount + fileCount1);
        }
    }
}
