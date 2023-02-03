using System;
using System.Linq;
using Clysh.Core;
using Clysh.Data;
using Clysh.Helper;
using Moq;
using NUnit.Framework;

namespace Clysh.Tests;

public class ClyshViewTests
{
    private readonly Mock<IClyshConsole> consoleMock = new();

    private readonly ClyshData metadata = new(title: "Auth 2 API", "1.0");

    [SetUp]
    public void Setup()
    {
        consoleMock.Reset();
    }

    [Test]
    public void SuccessfulConfirmWithYesAnswer()
    {
        var answerExpected = "Y";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (Y/n):";
        var answer = view.Confirm();

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsTrue(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithNoAnswer()
    {
        var answerExpected = "n";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (Y/n):";
        var answer = view.Confirm();

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsFalse(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithOtherAnswer()
    {
        var answerExpected = "xxxxx";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (Y/n):";
        var answer = view.Confirm();

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsFalse(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithEmptyAnswer()
    {
        var answerExpected = "";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (Y/n):";
        var answer = view.Confirm();

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsFalse(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithCustomYesAnswer()
    {
        var answerExpected = "I'm agree";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (I'm agree/n):";
        var answer = view.Confirm(yes: "I'm agree");

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsTrue(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithCustomNoAnswer()
    {
        var answerExpected = "NO";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Do you agree? (I'm agree/n):";
        var answer = view.Confirm(yes: "I'm agree");

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsFalse(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulConfirmWithCustomQuestion()
    {
        var answerExpected = "Y";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "Are you kidding me? (Y/n):";
        var answer = view.Confirm("Are you kidding me?");

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);
        consoleMock.Verify(x => x.ReadLine(), Times.Once);

        Assert.IsTrue(answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulAskFor()
    {
        var answerExpected = "test answer";
        consoleMock.Setup(x => x.ReadLine()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "test question:";
        var answer = view.AskFor("test question");

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);

        Assert.AreEqual(answerExpected, answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulAskForSensitive()
    {
        var answerExpected = "x1A";
        consoleMock.Setup(x => x.ReadSensitive()).Returns(answerExpected);

        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "test question:";
        var answer = view.AskForSensitive("test question");

        consoleMock.Verify(x => x.Write(question, 1), Times.Once);

        Assert.AreEqual(answerExpected, answer);
        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void AskForWithWhitespaceError()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var question = "     ";
        var exception = Assert.Throws<ArgumentException>(() => view.AskFor(question));

        ExtendedAssert.MatchMessage(exception?.Message, ClyshMessages.ErrorOnValidateUserInputQuestionAnswer);
        
        consoleMock.Verify(x => x.Write($"{question}:"), Times.Never);
        consoleMock.Verify(x => x.ReadLine(), Times.Never);
        Assert.AreEqual(0, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintRootCommandHelp()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        var command = ClyshDataForTest.CreateRootCommand();
        view.PrintHelp(command);

        consoleMock.Verify(x => x.WriteLine("", 1), Times.Once);
        consoleMock.Verify(x => x.WriteLine($"{metadata.Title}. Version: {metadata.Version}", 2), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 3), Times.Once);
        consoleMock.Verify(x => x.WriteLine("Usage: auth2 [options] [subcommands]", 4), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 5), Times.Once);
        consoleMock.Verify(x => x.WriteLine(command.Description, 6), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 7), Times.Once);
        consoleMock.Verify(x => x.WriteLine("[options]:", 8), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 9), Times.Once);
        consoleMock.Verify(x => x.WriteLine("".PadRight(3) + "Option".PadRight(22) + "Group".PadRight(11) + "Description".PadRight(35) + "Parameters", 10), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 11), Times.Once);

        var i = 0;

        foreach (var option in command.Options
                     .OrderBy(x => x.Value.Group?.Id)
                     .ThenBy(y=>y.Key)
                     .Select(z => z.Value))
        {
            var i1 = i;
            consoleMock.Verify(x => x.WriteLine("".PadRight(3) + $"{option,-22}{option.Group,-11}{option.Description,-35}{option.Parameters}", 12 + i1), Times.Once);
            i++;
        }

        //i=4

        consoleMock.Verify(x => x.WriteLine("", 12 + i), Times.Once);
        consoleMock.Verify(x => x.WriteLine("[subcommands]:", 13 + i), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 14 + i), Times.Once);

        var j = i + 1;
        foreach (var item in command.SubCommands.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value))
        {
            if (item.Key != command.Id)
            {
                var j1 = j;
                consoleMock.Verify(x => x.WriteLine("".PadRight(3) + $"{item.Value.Name,-39}{item.Value.Description}", 14 + j1), Times.Once);
                j++;
            }
        }

        //j=7

        consoleMock.Verify(x => x.WriteLine("", 14 + j), Times.Once);

        Assert.AreEqual(23, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintCustomCommandHelp()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        IClyshCommand command = ClyshDataForTest.CreateRootCommand().SubCommands["auth2.credential"];
        view.PrintHelp(command);

        consoleMock.Verify(x => x.WriteLine("", 1), Times.Once);
        consoleMock.Verify(x => x.WriteLine($"{metadata.Title}. Version: {metadata.Version}", 2), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 3), Times.Once);
        consoleMock.Verify(x => x.WriteLine("Usage: auth2 credential [options] [subcommands]", 4), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 5), Times.Once);
        consoleMock.Verify(x => x.WriteLine(command.Description, 6), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 7), Times.Once);
        consoleMock.Verify(x => x.WriteLine("[options]:", 8), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 9), Times.Once);
        consoleMock.Verify(x => x.WriteLine("".PadRight(3) + "Option".PadRight(22) + "Group".PadRight(11) + "Description".PadRight(35) + "Parameters", 10), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 11), Times.Once);

        var i = 0;

        foreach (var option in command.Options.OrderBy(x => x.Value.Group?.Id)
                     .ThenBy(y => y.Key)
                     .Select(z => z.Value))
        {
            var i1 = i;
            consoleMock.Verify(x => x.WriteLine("".PadRight(3) + $"{option,-33}{option.Description,-35}{option.Parameters}", 12 + i1), Times.Once);
            i++;
        }

        //i=4

        consoleMock.Verify(x => x.WriteLine("", 12 + i), Times.Once);
        consoleMock.Verify(x => x.WriteLine("[subcommands]:", 13 + i), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 14 + i), Times.Once);

        var j = i + 1;
        foreach (var item in command.SubCommands.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value))
        {
            if (item.Key != command.Id)
            {
                var j1 = j;
                consoleMock.Verify(x => x.WriteLine("".PadRight(3) + $"{item.Value.Name,-39}{item.Value.Description}", 14 + j1), Times.Once);
                j++;
            }
        }

        //j=7

        consoleMock.Verify(x => x.WriteLine("", 14 + j), Times.Once);

        Assert.AreEqual(21, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintCustomCommandTreeHelp()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        IClyshCommand command = ClyshDataForTest.CreateRootCommand().SubCommands["auth2.credential"].SubCommands["auth2.credential.test"];
        view.PrintHelp(command);

        consoleMock.Verify(x => x.WriteLine("", 1), Times.Once);
        consoleMock.Verify(x => x.WriteLine($"{metadata.Title}. Version: {metadata.Version}", 2), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 3), Times.Once);
        consoleMock.Verify(x => x.WriteLine("Usage: auth2 credential test [options]", 4), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 5), Times.Once);
        consoleMock.Verify(x => x.WriteLine(command.Description, 6), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 7), Times.Once);
        consoleMock.Verify(x => x.WriteLine("[options]:", 8), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 9), Times.Once);
        consoleMock.Verify(x => x.WriteLine("".PadRight(3) + "Option".PadRight(22) + "Group".PadRight(11) + "Description".PadRight(35) + "Parameters", 10), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 11), Times.Once);

        var i = 0;

        foreach (var option in command.Options.OrderBy(x => x.Value.Group?.Id)
                     .ThenBy(y => y.Key)
                     .Select(z => z.Value))
        {
            var i1 = i;
            var groupId = option.Group?.Id;
            var truncate = option.Description.Length > 30;
            var firstDescriptionLine = truncate ? option.Description[..30] : option.Description;
            consoleMock.Verify(x => x.WriteLine("".PadRight(3) + $"{option, -22}{groupId,-11}{firstDescriptionLine,-35}{option.Parameters}", 12 + i1), Times.Once);
            i++;
        }

        //i=4

        consoleMock.Verify(x => x.WriteLine("", 12 + i), Times.Once);

        Assert.AreEqual(16, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintRootCommandWithExceptionHelp()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata, true);

        try
        {
            throw new Exception("Test Exception");
        }
        catch (Exception exception)
        {
            view.PrintException(exception);
        }

        consoleMock.Verify(x => x.WriteLine("", 1), Times.Once);
        consoleMock.Verify(x => x.WriteLine($"Error: Test Exception", 2), Times.Once);
        consoleMock.Verify(x => x.WriteLine("", 3), Times.Once);
        
        Assert.AreEqual(3, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintWithoutLineNumber()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata);

        var question = "test question:";
        
        view.Print(question);

        consoleMock.Verify(x => x.WriteLine(question), Times.Once);

        Assert.AreEqual(1, view.PrintedLines);
    }

    [Test]
    public void SuccessfulPrintWithoutLineNumberAndNoBreak()
    {
        IClyshView view = new ClyshView(consoleMock.Object, metadata);

        var question = "test question:";
        
        view.PrintWithoutBreak(question);

        consoleMock.Verify(x => x.Write(question), Times.Once);

        Assert.AreEqual(1, view.PrintedLines);
    }
}