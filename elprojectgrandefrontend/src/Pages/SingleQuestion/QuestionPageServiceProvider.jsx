import {toast} from "react-hot-toast";

export const fetchAnswers = async (questionId, setAnswers) => {
    try {
        const res = await fetch('/api/Answers?questionId=' + questionId);
        const data = await res.json();
        if (data.message) {
            toast.error(data.message);
            return;
        }
        setAnswers(data.sort((a, b) => b.votes - a.votes));
    } catch (error) {
        console.log(error);
    }
}

export const postAnswer = async (questionId, cookies, reply, showErrorToast) => {
    try {

        const res = await fetch('/api/Answers?questionId=' + questionId, {
            method: "POST",
            headers: {
                'Authorization': "Bearer " + cookies.user,
                'Content-type': 'application/json'
            },
            body: JSON.stringify({content: reply.content, postedAt: new Date(Date.now()).toISOString()}),
        });
        return await res.json();
    } catch (error) {
        showErrorToast("something went wrong");
        console.error(error);
    }
}

export const formatTimeDifference = (postedAt) => {
    const now = new Date();
    const tempPostedAt = new Date(postedAt);
    let offset = tempPostedAt.getTimezoneOffset() * -1;
    const postedDate = new Date(tempPostedAt.getTime() + offset * 60000);
    let differenceInSeconds = Math.floor(((now - postedDate) / 1000));
    if (differenceInSeconds < 60) {
        return `${differenceInSeconds} second${differenceInSeconds === 1 ? '' : 's'} ago`;
    } else if (differenceInSeconds < 3600) {
        const minutes = Math.floor(differenceInSeconds / 60);
        return `${minutes} minute${minutes === 1 ? '' : 's'} ago`;
    } else if (differenceInSeconds < 86400) {
        const hours = Math.floor(differenceInSeconds / 3600);
        return `${hours} hour${hours === 1 ? '' : 's'} ago`;
    }
    const days = Math.floor(differenceInSeconds / 86400);
    return `${days} day${days === 1 ? '' : 's'} ago`;

};

export const handleAccept = async (id, cookies, setAnswers, setQuestionData) => {
    const res = await fetch('/api/accept/' + id, {
            method: 'POST',
            headers: {
                Authorization: "Bearer " + cookies.user
            }
        }
    );
    const data = await res.json();
    if (data.message) {
        toast.error(data.message);
        return;
    }
    if (data.accepted) {
        setAnswers((answers) => answers.map(ans => {
            if (ans.id === data.id) {
                return {...ans, accepted: true}
            }
            return ans;
        }).sort((a, b) => b.votes - a.votes));
        setQuestionData((questionData) => ({...questionData, hasAccepted: true}));
    }
}

export const sendUpvote = async (id, cookies) => {
    const res = await fetch('/api/Answers/' + id + '/upvote', {
        method: 'PATCH',
        headers: {
            Authorization: "Bearer " + cookies.user
        }
    })
    const data = await res.json();
    if (data.message) {
        toast.error(data.message);
        throw new Error();
    }
}


export const sendDownvote = async (id, cookies) => {

    const res = await fetch('/api/Answers/' + id + '/downvote', {
        method: 'PATCH',
        headers: {
            Authorization: "Bearer " + cookies.user
        }
    })
    const data = await res.json();
    if (data.message) {
        toast.error(data.message);
        throw new Error();
    }
}

export const checkIfUpvoted = (id, user) => {

    return user.upvotes.some(identifier => identifier === id);
}
export const checkIfDownvoted = (id, user) => {
    return user.downvotes.some(identifier => identifier === id);
}

export const handleUpvoting = async (answer, user, cookies, showErrorToast, setAnswers, setUser) => {
    if (checkIfUpvoted(answer.id, user)) {
        try {
            await sendUpvote(answer.id, cookies);
        } catch (e) {
            console.log(e);
            return;
        }
        setAnswers(prevAnswers =>
            prevAnswers.map(ans =>
                ans.id === answer.id ? {...ans, votes: ans.votes - 1} : ans
            ).sort((a, b) => b.votes - a.votes)
        );

        setUser(prevUser => ({
            ...prevUser,
            upvotes: prevUser.upvotes.filter(answerId => answerId !== answer.id)
        }));

    } else {
        try {
            await sendUpvote(answer.id, cookies);
        } catch (error) {
            console.log(error);
            return;
        }
        setAnswers(prevAnswers =>
            prevAnswers.map(ans =>
                ans.id === answer.id ? {
                    ...ans,
                    votes: checkIfDownvoted(answer.id, user) ? ans.votes + 2 : ans.votes + 1
                } : ans
            ).sort((a, b) => b.votes - a.votes)
        );

        setUser(prevUser => ({
            ...prevUser,
            upvotes: [...prevUser.upvotes, answer.id],
            downvotes: prevUser.downvotes.filter(ide => ide !== answer.id)
        }));
    }
};

export const handleDownvoting = async (answer, user, cookies, showErrorToast, setAnswers, setUser) => {
    if (checkIfDownvoted(answer.id, user)) {
        try {
            await sendDownvote(answer.id, cookies);
        } catch (e) {
            console.log(e);
            return;
        }

        setAnswers(prevAnswers =>
            prevAnswers.map(ans =>
                ans.id === answer.id ? {...ans, votes: ans.votes + 1} : ans
            ).sort((a, b) => b.votes - a.votes)
        );

        setUser(prevUser => ({
            ...prevUser,
            downvotes: prevUser.downvotes.filter(ansId => ansId !== answer.id)
        }));
    } else {
        try {
            await sendDownvote(answer.id, cookies);
        } catch (e) {
            console.log(e);
            return;
        }

        setAnswers(prevAnswers =>
            prevAnswers.map(ans =>
                ans.id === answer.id ? {
                    ...ans,
                    votes: checkIfUpvoted(answer.id, user) ? ans.votes - 2 : ans.votes - 1
                } : ans
            ).sort((a, b) => b.votes - a.votes)
        );

        setUser(prevUser => ({
            ...prevUser,
            downvotes: [...prevUser.downvotes, answer.id],
            upvotes: prevUser.upvotes.filter(ansId => ansId !== answer.id)
        }));
    }
};

