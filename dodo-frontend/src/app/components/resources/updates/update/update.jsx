import { useTranslation } from "react-i18next";
import React, { useCallback, useEffect, useState } from "react";
import styles from "./update.module.scss";
import { UpdateMeta } from "../update-meta";
import nounAttention from "static/noun_attention_2913340.png";
import nounCalendar from "static/noun_Calendar_3368739.png";
import nounSpeechBubble from "static/noun_Speech Bubble_1588036.png";
import twitterLogoBlue from "static/Twitter_Logo_Blue.svg";
import telegramLogo from "static/LRDASku7_400x400.png";

const UPDATE_TEXT_READ_MORE_LENGTH = 100;
const UPDATE_TYPE_ICON_MAP = {
	alert: nounAttention,
	calendar: nounCalendar,
	announcement: nounSpeechBubble,
	twitter: twitterLogoBlue,
	telegram: telegramLogo,
};

export const Update = ({
	notification: {
		canDelete,
		guid,
		link,
		message,
		permissionLevel,
		source,
		timestamp,
		type,
	},
}) => {
	const { t } = useTranslation("ui");

	const [messageContent, setMessageContent] = useState(message);
	const [showMoreEnabled, setShowMoreEnabled] = useState(false);
	const [showMore, setShowMore] = useState(false);

	const setTrimmedMessage = useCallback(
		() =>
			setMessageContent(
				messageContent.substring(0, UPDATE_TEXT_READ_MORE_LENGTH) +
					"..."
			),
		[messageContent]
	);

	useEffect(() => {
		if (messageContent?.length > UPDATE_TEXT_READ_MORE_LENGTH) {
			setTrimmedMessage();
			setShowMoreEnabled(true);
		}
	}, [message, messageContent, setTrimmedMessage]);

	const onShowMoreClick = (event) => {
		event.preventDefault();
		if (!showMore) {
			setShowMore(true);
			setMessageContent(message);
		} else {
			setShowMore(false);
			setTrimmedMessage();
		}
	};

	return (
		<div className={styles.update} key={guid}>
			<div className={styles.updateContent}>
				<UpdateMeta timestamp={timestamp} />
				<div
					className={styles.message}
					dangerouslySetInnerHTML={{ __html: messageContent }}
				/>
				<div className={styles.updateActions}>
					{showMoreEnabled ? (
						<button
							className={styles.showMoreUpdate}
							onClick={onShowMoreClick}
						>
							{showMore
								? t("notifications_show_less")
								: t("notifications_show_more")}
						</button>
					) : null}
				</div>
			</div>
			<div className={styles.updateIcon}>
				<img
					src={UPDATE_TYPE_ICON_MAP[type]}
					alt={t("header_logo_alt")}
					className={styles.updateIconImg}
				/>
			</div>
		</div>
	);
};
